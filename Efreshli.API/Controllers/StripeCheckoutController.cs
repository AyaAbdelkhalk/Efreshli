//using Efreshli.Application.DTOs.OrderDTOs;
//using Efreshli.Application.Helper.ResultPattern;
//using Efreshli.Application.Services.CartServices;
//using Efreshli.Application.Services.OrderServices;
//using Efreshli.Domain.Common.Interfaces;
//using Efreshli.Domain.Enums;
//using Efreshli.Domain.Models;
//using Efreshli.Infrastructure.Repositories;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Stripe;
//using Stripe.Checkout;
//using System.Globalization;
//using System.Security.Claims;
//using System.Text;

//[Route("api/payments")]
//[ApiController]
//public class StripeCheckoutController : ControllerBase
//{
//    private readonly IConfiguration _config;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ICartService _cartService;
//    private readonly IOrderService _orderService;
//    public StripeCheckoutController(IConfiguration config, IUnitOfWork uow, ICartService cartService, IOrderService orderService)
//    {
//        _config = config;
//        _unitOfWork = uow;
//        _cartService = cartService;
//        _orderService = orderService;
//    }

//    [HttpPost("create-checkout-session")]
//    public async Task<Response<string>> CreateStripeCheckoutSessionAsync(string userId, CreateOrderDto createOrderDto)
//    {
//        // 1. Get checkout preview with coupon applied
//        var checkoutPreviewResponse = await _orderService.GetCheckoutPreviewAsync(userId, createOrderDto.CouponId);

//        if (!checkoutPreviewResponse.Succeeded || checkoutPreviewResponse.Data == null)
//            return ResponseHandler.BadRequest<string>("Unable to calculate checkout preview");

//        var preview = checkoutPreviewResponse.Data;

//        // 2. Initialize Stripe
//        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

//        var options = new SessionCreateOptions
//        {
//            PaymentMethodTypes = new List<string> { "card" },
//            Mode = "payment",

//            LineItems = new List<SessionLineItemOptions>
//        {
//            new SessionLineItemOptions
//            {
//                PriceData = new SessionLineItemPriceDataOptions
//                {
//                    Currency = "usd",
//                    UnitAmountDecimal = preview.TotalPrice * 100, // Stripe needs cents
//                    ProductData = new SessionLineItemPriceDataProductDataOptions
//                    {
//                        Name = "Efreshli Order Checkout",
//                        Description = $"Subtotal: {preview.SubTotalPrice}, Discount: {preview.DiscountValue ?? 0}"
//                    },
//                },
//                Quantity = 1
//            }
//        },

//            SuccessUrl = $"{_config["App:ClientUrl"]}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
//            CancelUrl = $"{_config["App:ClientUrl"]}/checkout/cancel",
//            CustomerEmail = (await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(u => u.Id == userId))?.Email,
//        };

//        var service = new SessionService();
//        var session = await service.CreateAsync(options);

//        // 3. Save payment entry (status = Pending)
//        var payment = new Payment
//        {
//            ApplicationUserId = userId,
//            Amount = preview.TotalPrice,
//            PaymentMethod = createOrderDto.PaymentMethod,
//            PaymentStatus = PaymentStatus.Pending,
//            TransactionId = session.Id
//        };

//        await _unitOfWork.PaymentRepository.AddAsync(payment);
//        await _unitOfWork.SaveChangesAsync();

//        return ResponseHandler.Success(session.Url, "Stripe checkout session created");
//    }

//    // Webhook: create Order + Payment on successful checkout (idempotent)
//    [HttpPost("webhook")]
//    public async Task<IActionResult> Webhook()
//    {
//        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
//        var webhookSecret = _config["Stripe:WebhookSecret"];
//        Event stripeEvent;

//        try
//        {
//            var signature = Request.Headers["Stripe-Signature"];
//            stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
//        }
//        catch (Exception e)
//        {
//            return BadRequest($"Webhook signature verification failed: {e.Message}");
//        }

//        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
//        {
//            var session = stripeEvent.Data.Object as Session;
//            if (session != null)
//            {
//                await HandleCheckoutSessionCompleted(session);
//            }
//        }

//        return Ok();
//    }

//    private async Task HandleCheckoutSessionCompleted(Session session)
//    {
//        // Idempotency: check if we've already processed this payment (use PaymentIntentId or session.Id)
//        var paymentIntentId = session.PaymentIntentId ?? session.Id;
//        var existingPayment = await _unitOfWork.PaymentRepository.GetFirstOrDefaultAsync(p => p.TransactionId == paymentIntentId);
//        if (existingPayment != null)
//        {
//            // already processed
//            return;
//        }

//        // metadata
//        session.Metadata.TryGetValue("userId", out var userId);
//        session.Metadata.TryGetValue("addressId", out var addressIdStr);
//        session.Metadata.TryGetValue("couponCode", out var couponCode);
//        session.Metadata.TryGetValue("paymentMethod", out var paymentMethodStr);
//        session.Metadata.TryGetValue("note", out var note);
//        session.Metadata.TryGetValue("deliveryNotes", out var deliveryNotes);

//        if (string.IsNullOrEmpty(userId)) return;

//        // Create order and order items from cart server-side (do not trust client)
//        var cart = await _unitOfWork.CartRepository.GetAsync(c => c.ApplicationUserId == userId, include: "Items,Items.Product");
//        if (cart == null)
//        {
//            // nothing to create
//            return;
//        }

//        // Compute totals again server-side (apply coupon if present)
//        decimal cartTotal = await _cartService.GetGrandTotalOfCart(userId);
//        decimal discountFixed = 0m;
//        decimal discountPercent = 0m;
//        Coupon? coupon = null;
//        if (!string.IsNullOrWhiteSpace(couponCode))
//        {
//            coupon = await _unitOfWork.CouponRepository.SingleOrDefaultAsync(c => c.Code == couponCode);
//            if (coupon != null && coupon.IsActive && (!coupon.ExpiryDate.HasValue || coupon.ExpiryDate.Value >= DateTime.UtcNow))
//            {
//                if (coupon.Type == CouponType.Percentage)
//                    discountPercent = coupon.Value / 100m;
//                else
//                    discountFixed = coupon.Value;
//            }
//        }

//        // create order
//        var order = new Order
//        {
//            ApplicationUserId = userId,
//            AddressId = int.TryParse(addressIdStr, out var aId) ? aId : 0,
//            Note = note,
//            DeliveryNotes = deliveryNotes,
//            CreatedAt = DateTime.UtcNow,
//            Status = OrderStatus.Completed, // or Paid/Confirmed depending on your enums
//            Total = 0m
//        };

//        // Add items and calculate order total applying coupon proportionally
//        decimal orderTotal = 0m;
//        foreach (var ci in cart.Items)
//        {
//            decimal itemTotal = ci.UnitPrice * ci.Quantity;
//            decimal itemDiscount = 0m;
//            if (discountPercent > 0) itemDiscount = itemTotal * discountPercent;
//            else if (discountFixed > 0) itemDiscount = (itemTotal / cartTotal) * discountFixed;

//            decimal finalItemTotal = Math.Max(0m, itemTotal - itemDiscount);

//            var orderItem = new OrderItem
//            {
//                ProductId = ci.ProductId,
//                Quantity = ci.Quantity,
//                UnitPrice = ci.UnitPrice,
//                Total = finalItemTotal
//            };
//            order.OrderItems.Add(orderItem);
//            orderTotal += finalItemTotal;
//        }

//        order.Total = orderTotal;

//        await _unitOfWork.OrderRepository.AddAsync(order);
//        await _unitOfWork.SaveChangesAsync();

//        // create payment record
//        var payment = new Payment
//        {
//            ApplicationUserId = userId,
//            Order = order,
//            Amount = orderTotal,
//            PaymentMethod = MapToPaymentMethod(paymentMethodStr),
//            TransactionId = paymentIntentId,
//            PaymentStatus = PaymentStatus.Paid,
//            DeliveryNotes = deliveryNotes,
//            CreatedAt = DateTime.UtcNow
//        };

//        await _unitOfWork.PaymentRepository.AddAsync(payment);

//        // clear cart
//        foreach (var item in cart.Items.ToList())
//            _unitOfWork.CartItemRepository.Delete(item);
//        _unitOfWork.CartRepository.Delete(cart);

//        await _unitOfWork.SaveChangesAsync();
//    }

//    // Helper to map string to your Domain.Enums.PaymentMethod
//    private Domain.Enums.PaymentMethod MapToPaymentMethod(string paymentMethodStr)
//    {
//        if (Enum.TryParse<Domain.Enums.PaymentMethod>(paymentMethodStr, true, out var pm))
//            return pm;
//        return Domain.Enums.PaymentMethod.CreditCard; // fallback
//    }

//    // Endpoint the Angular success page can call to get order by session id
//    [HttpGet("session-order")]
//    public async Task<IActionResult> GetOrderBySession([FromQuery] string sessionId)
//    {
//        if (string.IsNullOrWhiteSpace(sessionId)) return BadRequest("sessionId required");

//        // find payment by transaction id which we saved as PaymentIntentId (or fallback to session id)
//        var payment = await _unitOfWork.PaymentRepository.SingleOrDefaultAsync(p => p.TransactionId == sessionId);
//        if (payment == null)
//        {
//            // maybe the webhook is still processing
//            return NotFound();
//        }

//        return Ok(new { orderId = payment.Order?.OrderId, paymentId = payment.PaymentId });
//    }
//}