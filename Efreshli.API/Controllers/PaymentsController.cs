using Efreshli.Application.Services.CartServices;
using Efreshli.Application.Services.OrderServices;
using Efreshli.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderService _orderService; // optionally to compute preview
    private readonly ICartService _cartService; // or ICartService
    private readonly IUserContext _userContext;
    public PaymentsController(IConfiguration config, IUnitOfWork unitOfWork, ICartService cartService,IUserContext userContext)

    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        _unitOfWork = unitOfWork;
        _cartService = cartService;
        _userContext = userContext;
    }

    public class CreateCheckoutSessionRequest
    {
        //public string UserId { get; set; } = default!;
        public int AddressId { get; set; }
        public string? CouponCode { get; set; }
        public string SuccessUrl { get; set; } = default!; // e.g. https://app.example.com/checkout-success
        public string CancelUrl { get; set; } = default!; // e.g. https://app.example.com/checkout-cancel
    }
    [Authorize]
    //[AllowAnonymous]
    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
    {
        if (string.IsNullOrEmpty(_userContext.CurrentUserId))
            return Unauthorized();

        // 1. Recompute cart and verify items exist & quantities
        var cartResp = await _cartService.GetCartByUserIdAsync(_userContext.CurrentUserId);
        if (!cartResp.Succeeded || cartResp.Data == null || !cartResp.Data.Items.Any())
            return BadRequest("Cart is empty or invalid.");

        // Calculate subtotal (use server-side reliable values)
        decimal subTotal = cartResp.Data.GrandTotal;
        decimal discount = 0;
        // Optional: validate coupon server side (reuse your coupon logic)
       Efreshli.Domain.Models.Coupon? coupon = null;
        if (!string.IsNullOrWhiteSpace(req.CouponCode))
        {
            coupon = await _unitOfWork.CouponRepository.GetAll().FirstOrDefaultAsync(c => c.Code == req.CouponCode);
            if (coupon != null && coupon.IsActive && DateOnly.FromDateTime(coupon.ExpireDate) >= DateOnly.FromDateTime(DateTime.Now))
            {
                if (subTotal >= (coupon.MinOrderAmount ?? 0))
                {
                    discount = coupon.IsPercentage ? (subTotal * coupon.DiscountValue / 100m) : coupon.DiscountValue;
                }
            }
        }

        decimal shipping = 50m; // your constant
        long amountInCents = (long)((subTotal - discount + shipping) * 100m);

        // Build Stripe line items - for simplicity, a single line item for total amount
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = amountInCents,
                        Currency = "usd", // adapt to your currency
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Order payment",
                            Description = $"User {_userContext.CurrentUserId} order"
                        }
                    }
                }
            },

            //SuccessUrl = req.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
            //CancelUrl = req.CancelUrl,
            SuccessUrl = req.SuccessUrl?? "http://localhost:4200/checkout-success" + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = req.CancelUrl?? "http://localhost:4200/checkout-cancel",
            // Save important metadata for the webhook
            Metadata = new Dictionary<string, string>
            {
                { "userId",_userContext.CurrentUserId },
                { "addressId", req.AddressId.ToString() },
                { "couponCode", req.CouponCode?.ToUpper() ?? "" }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        // Return session url or id to the frontend
        return Ok(new { sessionId = session.Id, url = session.Url });
    }
}
