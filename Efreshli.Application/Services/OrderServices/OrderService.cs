using Efreshli.Application.DTOs.OrderDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Helper.Pagination;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Mapster;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace Efreshli.Application.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly IUserContext _userContext;
        private const decimal DEFAULT_SHIPPING_PRICE = 50.0m;
        private const int DEFAULT_DELIVERY_DAYS = 3;

        public OrderService(IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<Response<Order>> CreateOrderFromStripeSessionAsync(Session session)
        {
            // 1. Extract metadata
            var userId = session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null;
            var addressIdStr = session.Metadata.ContainsKey("addressId") ? session.Metadata["addressId"] : null;
            var couponCode = session.Metadata.ContainsKey("couponCode") ? session.Metadata["couponCode"] : null;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(addressIdStr) || !int.TryParse(addressIdStr, out var addressId))
                return ResponseHandler.BadRequest<Order>("Invalid session metadata");

            // 2. Idempotency check
            var existingPayment = await _unitOfWork.PaymentRepository.GetAll()
                .FirstOrDefaultAsync(p => p.StripeSessionId == session.Id);

            if (existingPayment != null)
                return ResponseHandler.Success(existingPayment.Order, "Order already created for this session.");

            // 3. Execute everything in a transaction
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // 1. Validate cart
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[] { c => c.Items }
                );

                var userCart = carts.FirstOrDefault();
                if (userCart == null || !userCart.Items.Any())
                    return ResponseHandler.BadRequest<Order>("Cart is empty.");

                // 2. Validate address
                var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
                if (address == null || address.ApplicationUserId != userId)
                    return ResponseHandler.BadRequest<Order>("Invalid shipping address.");

                // 3. Calculate totals & prepare order items
                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in userCart.Items)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
                    if (productItem == null)
                        return ResponseHandler.NotFound<Order>($"Product item {cartItem.ProductItemId} not found");

                    if (productItem.Quantity < cartItem.RequiredQuantity)
                        return ResponseHandler.BadRequest<Order>($"Insufficient stock for {productItem.ProductItemId}");

                    var itemTotal = productItem.Price * cartItem.RequiredQuantity;
                    subTotal += itemTotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductItemId = cartItem.ProductItemId,
                        Quantity = cartItem.RequiredQuantity,
                        Price = productItem.Price
                    });
                }

                // 4. Validate coupon (don’t consume yet)
                decimal discountValue = 0;
                Coupon? coupon = null;

                if (!string.IsNullOrEmpty(couponCode))
                {
                    coupon = await _unitOfWork.CouponRepository.GetAll()
                        .FirstOrDefaultAsync(c => c.Code== couponCode.ToUpper());

                    if (coupon != null && coupon.IsActive && coupon.ExpireDate >= DateTime.UtcNow
                        && coupon.UsedCount < coupon.UsageLimit && subTotal >= (coupon.MinOrderAmount ?? 0))
                    {
                        discountValue = coupon.IsPercentage
                            ? subTotal * coupon.DiscountValue / 100
                            : coupon.DiscountValue;
                    }
                }

                // 5. Create Order (payment is already done in Stripe)
                var order = new Order
                {
                    ApplicationUserId = userId,
                    SubTotalPrice = subTotal,
                    DiscountValue = discountValue > 0 ? discountValue : null,
                    ShippingPrice = DEFAULT_SHIPPING_PRICE,
                    TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
                    EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(DEFAULT_DELIVERY_DAYS)),
                    CouponId = coupon?.CouponId,
                    Status = OrderStatus.Pending,
                    AddressId = addressId,
                    OrderItems = orderItems,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.OrderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // 6. Record payment from Stripe session
                var payment = new Payment
                {
                    ApplicationUserId = userId,
                    Amount = order.TotalPrice,
                    PaymentMethod = PaymentMethod.Stripe,
                    PaymentStatus = PaymentStatus.Paid, // already paid in Stripe
                    StripeSessionId = session.Id,
                    StripePaymentIntentId = session.PaymentIntentId,
                    TransactionId = session.PaymentIntentId,
                    OrderId = order.OrderId
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Link payment in order
                order.PaymentId = payment.PaymentId;
                await _unitOfWork.SaveChangesAsync();

                // 7. Reduce stock
                foreach (var cartItem in userCart.Items)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
                    if (productItem != null)
                    {
                        productItem.Quantity -= cartItem.RequiredQuantity;
                        await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
                    }
                }

                // 8. Clear cart
                foreach (var item in userCart.Items.ToList())
                {
                    await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);
                }
                
                // 9. Consume coupon (after successful order + stock update)
                if (coupon != null && discountValue > 0)
                {
                    var user =await _unitOfWork.UserRepository.GetAll().Include(x=>x.Coupons).FirstOrDefaultAsync(x => x.Id == userId);
                    user.Coupons.Add(coupon);
                    
                    coupon.UsedCount++;
                    await _unitOfWork.CouponRepository.UpdateAsync(coupon, CancellationToken.None);
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                }

                await _unitOfWork.SaveChangesAsync();

                return ResponseHandler.Created(order);
            });




            //return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            //{
            //    // Validate cart
            //    var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
            //        c => c.ApplicationUserId == userId,
            //        includes: new Expression<Func<Cart, object>>[] { c => c.Items }
            //    );

            //    var userCart = carts.FirstOrDefault();
            //    if (userCart == null || !userCart.Items.Any())
            //        return ResponseHandler.BadRequest<Order>("Cart is empty.");

            //    // Validate address
            //    var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
            //    if (address == null || address.ApplicationUserId != userId)
            //        return ResponseHandler.BadRequest<Order>("Invalid shipping address.");

            //    // Calculate totals & create OrderItems
            //    decimal subTotal = 0;
            //    var orderItems = new List<OrderItem>();

            //    foreach (var cartItem in userCart.Items)
            //    {
            //        var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
            //        if (productItem == null)
            //            return ResponseHandler.NotFound<Order>($"Product item {cartItem.ProductItemId} not found");

            //        if (productItem.Quantity < cartItem.RequiredQuantity)
            //            return ResponseHandler.BadRequest<Order>($"Insufficient stock for {productItem.ProductItemId}");

            //        var itemTotal = productItem.Price * cartItem.RequiredQuantity;
            //        subTotal += itemTotal;

            //        orderItems.Add(new OrderItem
            //        {
            //            ProductItemId = cartItem.ProductItemId,
            //            Quantity = cartItem.RequiredQuantity,
            //            Price = productItem.Price
            //        });
            //    }

            //    // Handle coupon
            //    decimal discountValue = 0;
            //    Coupon? coupon = null;

            //    if (!string.IsNullOrEmpty(couponCode))
            //    {
            //        coupon = await _unitOfWork.CouponRepository.GetAll()
            //            .FirstOrDefaultAsync(c => c.Code == couponCode);

            //        if (coupon != null && coupon.IsActive && coupon.ExpireDate >= DateTime.UtcNow
            //            && coupon.UsedCount < coupon.UsageLimit && subTotal >= (coupon.MinOrderAmount ?? 0))
            //        {
            //            discountValue = coupon.IsPercentage
            //                ? subTotal * coupon.DiscountValue / 100
            //                : coupon.DiscountValue;

            //            coupon.UsedCount++;
            //            await _unitOfWork.CouponRepository.UpdateAsync(coupon, CancellationToken.None);
            //        }
            //    }

            //    // Create Order
            //    var order = new Order
            //    {
            //        ApplicationUserId = userId,
            //        SubTotalPrice = subTotal,
            //        DiscountValue = discountValue > 0 ? discountValue : null,
            //        ShippingPrice = DEFAULT_SHIPPING_PRICE,
            //        TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
            //        EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(DEFAULT_DELIVERY_DAYS)),
            //        CouponId = coupon?.CouponId,
            //        Status = OrderStatus.Pending,
            //        AddressId = addressId,
            //        OrderItems = orderItems,
            //        CreatedDate = DateTime.UtcNow
            //    };

            //    await _unitOfWork.OrderRepository.AddAsync(order);
            //    await _unitOfWork.SaveChangesAsync(); // <-- يحفظ OrderId

            //    // Create Payment
            //    var payment = new Payment
            //    {
            //        ApplicationUserId = userId,
            //        Amount = order.TotalPrice,
            //        PaymentMethod = PaymentMethod.Stripe,
            //        PaymentStatus = PaymentStatus.Paid,
            //        StripeSessionId = session.Id,
            //        StripePaymentIntentId = session.PaymentIntentId,
            //        TransactionId = session.PaymentIntentId,
            //        OrderId = order.OrderId // ربط الدفع بالطلب بعد الحفظ
            //    };

            //    await _unitOfWork.PaymentRepository.AddAsync(payment);
            //    await _unitOfWork.SaveChangesAsync();

            //    // Link payment in Order
            //    order.PaymentId = payment.PaymentId;
            //    await _unitOfWork.SaveChangesAsync(); // لا حاجة لـ UpdateAsync

            //    // Update inventory
            //    foreach (var cartItem in userCart.Items)
            //    {
            //        var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
            //        if (productItem != null)
            //        {
            //            productItem.Quantity -= cartItem.RequiredQuantity;
            //            await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
            //        }
            //    }

            //    // Clear cart
            //    foreach (var item in userCart.Items.ToList())
            //    {
            //        await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);
            //    }

            //    await _unitOfWork.SaveChangesAsync();

            //    return ResponseHandler.Created(order);
            //});
        }

        //v2
        //public async Task<Response<Order>> CreateOrderFromStripeSessionAsync(Session session)
        //{
        //    // 1. Extract metadata
        //    var userId = session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null;
        //    var addressIdStr = session.Metadata.ContainsKey("addressId") ? session.Metadata["addressId"] : null;
        //    var couponCode = session.Metadata.ContainsKey("couponCode") ? session.Metadata["couponCode"] : null;

        //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(addressIdStr) || !int.TryParse(addressIdStr, out var addressId))
        //        return ResponseHandler.BadRequest<Order>("Invalid session metadata");

        //    try
        //    {
        //        // 2. Execute all operations in a single retryable transaction
        //        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        //        {
        //            // 2a. Idempotency check
        //            var existingPayment = await _unitOfWork.PaymentRepository.GetAll()
        //                .FirstOrDefaultAsync(p => p.StripeSessionId == session.Id);

        //            if (existingPayment != null)
        //                return ResponseHandler.Success(existingPayment.Order, "Order already created for this session.");

        //            // 2b. Validate cart & address
        //            var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
        //                c => c.ApplicationUserId == userId,
        //                includes: new Expression<Func<Cart, object>>[] { c => c.Items }
        //            );
        //            var userCart = carts.FirstOrDefault();

        //            if (userCart == null || userCart.Items == null || !userCart.Items.Any())
        //                return ResponseHandler.BadRequest<Order>("Cart is empty.");

        //            var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
        //            if (address == null || address.ApplicationUserId != userId)
        //                return ResponseHandler.BadRequest<Order>("Invalid shipping address.");

        //            // 2c. Prepare order items
        //            decimal subTotal = 0;
        //            var orderItems = new List<OrderItem>();

        //            foreach (var cartItem in userCart.Items)
        //            {
        //                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
        //                if (productItem == null)
        //                    return ResponseHandler.NotFound<Order>($"Product item {cartItem.ProductItemId} not found");

        //                if (productItem.Quantity < cartItem.RequiredQuantity)
        //                    return ResponseHandler.BadRequest<Order>($"Insufficient stock for {productItem.ProductItemId}");

        //                var itemTotal = productItem.Price * cartItem.RequiredQuantity;
        //                subTotal += itemTotal;

        //                orderItems.Add(new OrderItem
        //                {
        //                    ProductItemId = cartItem.ProductItemId,
        //                    Quantity = cartItem.RequiredQuantity,
        //                    Price = productItem.Price
        //                });
        //            }

        //            // 2d. Apply coupon if exists
        //            decimal discountValue = 0;
        //            Coupon? coupon = null;
        //            if (!string.IsNullOrEmpty(couponCode))
        //            {
        //                coupon = await _unitOfWork.CouponRepository.GetAll()
        //                    .FirstOrDefaultAsync(c => c.Code == couponCode);

        //                if (coupon != null && coupon.IsActive && coupon.ExpireDate >= DateTime.UtcNow &&
        //                    coupon.UsedCount < coupon.UsageLimit && subTotal >= (coupon.MinOrderAmount ?? 0))
        //                {
        //                    discountValue = coupon.IsPercentage ? subTotal * coupon.DiscountValue / 100 : coupon.DiscountValue;
        //                    coupon.UsedCount++;
        //                    await _unitOfWork.CouponRepository.UpdateAsync(coupon, CancellationToken.None);
        //                }
        //            }

        //            // 2e. Create Order
        //            var order = new Order
        //            {
        //                ApplicationUserId = userId,
        //                SubTotalPrice = subTotal,
        //                DiscountValue = discountValue > 0 ? discountValue : null,
        //                ShippingPrice = DEFAULT_SHIPPING_PRICE,
        //                TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
        //                EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(DEFAULT_DELIVERY_DAYS)),
        //                CouponId = coupon?.CouponId,
        //                Status = OrderStatus.Pending,
        //                AddressId = addressId,
        //                OrderItems = orderItems,
        //                CreatedDate = DateTime.UtcNow
        //            };

        //            await _unitOfWork.OrderRepository.AddAsync(order);

        //            // 2f. Create Payment
        //            var payment = new Payment
        //            {
        //                ApplicationUserId = userId,
        //                Amount = order.TotalPrice,
        //                PaymentMethod = PaymentMethod.Stripe,
        //                PaymentStatus = PaymentStatus.Paid,
        //                StripeSessionId = session.Id,
        //                StripePaymentIntentId = session.PaymentIntentId,
        //                TransactionId = session.PaymentIntentId
        //            };

        //            await _unitOfWork.PaymentRepository.AddAsync(payment, CancellationToken.None);

        //            // Link payment to order
        //            order.PaymentId = payment.PaymentId;
        //            await _unitOfWork.OrderRepository.UpdateAsync(order, CancellationToken.None);

        //            // 2g. Update inventory
        //            foreach (var cartItem in userCart.Items)
        //            {
        //                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
        //                if (productItem != null)
        //                {
        //                    productItem.Quantity -= cartItem.RequiredQuantity;
        //                    await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
        //                }
        //            }

        //            // 2h. Clear cart
        //            foreach (var item in userCart.Items.ToList())
        //                await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);

        //            // كل التغييرات يتم حفظها داخل ExecuteInTransactionAsync
        //            return ResponseHandler.Created(order);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseHandler.BadRequest<Order>(ex.Message);
        //    }
        //}
        //v1
        //public async Task<Response<Order>> CreateOrderFromStripeSessionAsync(Session session)
        //{
        //    // 1. Extract metadata
        //    var userId = session.Metadata.ContainsKey("userId") ? session.Metadata["userId"] : null;
        //    var addressIdStr = session.Metadata.ContainsKey("addressId") ? session.Metadata["addressId"] : null;
        //    var couponCode = session.Metadata.ContainsKey("couponCode") ? session.Metadata["couponCode"] : null;

        //    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(addressIdStr) || !int.TryParse(addressIdStr, out var addressId))
        //    {
        //        return ResponseHandler.BadRequest<Order>("Invalid session metadata");
        //    }

        //    // 2. Idempotency check
        //    var existingPayment = await _unitOfWork.PaymentRepository.GetAll()
        //        .FirstOrDefaultAsync(p => p.StripeSessionId == session.Id);

        //    if (existingPayment != null)
        //    {
        //        return ResponseHandler.Success(existingPayment.Order, "Order already created for this session.");
        //    }

        //    // 3. Validate cart & address
        //    var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
        //        c => c.ApplicationUserId == userId,
        //        includes: new Expression<Func<Cart, object>>[] { c => c.Items }
        //    );

        //    var userCart = carts.FirstOrDefault();
        //    if (userCart == null || userCart.Items == null || !userCart.Items.Any())
        //    {
        //        return ResponseHandler.BadRequest<Order>("Cart is empty.");
        //    }

        //    var address = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
        //    if (address == null || address.ApplicationUserId != userId)
        //    {
        //        return ResponseHandler.BadRequest<Order>("Invalid shipping address.");
        //    }

        //    // 4. Start transaction
        //    using var transaction = await _unitOfWork.BeginTransactionAsync();
        //    var strategy = _dbContext.Database.CreateExecutionStrategy();

        //    try
        //    {
        //        decimal subTotal = 0;
        //        var orderItems = new List<OrderItem>();

        //        foreach (var cartItem in userCart.Items)
        //        {
        //            var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
        //            if (productItem == null)
        //                return ResponseHandler.NotFound<Order>($"Product item {cartItem.ProductItemId} not found");

        //            if (productItem.Quantity < cartItem.RequiredQuantity)
        //                return ResponseHandler.BadRequest<Order>($"Insufficient stock for {productItem.ProductItemId}");

        //            var itemTotal = productItem.Price * cartItem.RequiredQuantity;
        //            subTotal += itemTotal;

        //            orderItems.Add(new OrderItem
        //            {
        //                ProductItemId = cartItem.ProductItemId,
        //                Quantity = cartItem.RequiredQuantity,
        //                Price = productItem.Price
        //            });
        //        }

        //        // 5. Handle coupon
        //        decimal discountValue = 0;
        //        Coupon? coupon = null;

        //        if (!string.IsNullOrEmpty(couponCode))
        //        {
        //            coupon = await _unitOfWork.CouponRepository.GetAll()
        //                .FirstOrDefaultAsync(c => c.Code == couponCode);

        //            if (coupon != null
        //                && coupon.IsActive
        //                && coupon.ExpireDate >= DateTime.UtcNow
        //                && coupon.UsedCount < coupon.UsageLimit
        //                && subTotal >= (coupon.MinOrderAmount ?? 0))
        //            {
        //                discountValue = coupon.IsPercentage
        //                    ? subTotal * coupon.DiscountValue / 100
        //                    : coupon.DiscountValue;

        //                coupon.UsedCount++;
        //                await _unitOfWork.CouponRepository.UpdateAsync(coupon, CancellationToken.None);
        //            }
        //        }

        //        // 6. Create Order
        //        var order = new Order
        //        {
        //            ApplicationUserId = userId,
        //            SubTotalPrice = subTotal,
        //            DiscountValue = discountValue > 0 ? discountValue : null,
        //            ShippingPrice = DEFAULT_SHIPPING_PRICE,
        //            TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
        //            EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(DEFAULT_DELIVERY_DAYS)),
        //            CouponId = coupon?.CouponId,
        //            Status = OrderStatus.Pending,
        //            AddressId = addressId,
        //            OrderItems = orderItems,
        //            CreatedDate = DateTime.UtcNow
        //        };

        //        await _unitOfWork.OrderRepository.AddAsync(order);
        //        await _unitOfWork.SaveChangesAsync();

        //        // 7. Create Payment
        //        var payment = new Payment
        //        {
        //            ApplicationUserId = userId,
        //            Amount = order.TotalPrice,
        //            PaymentMethod = PaymentMethod.Stripe,
        //            PaymentStatus = PaymentStatus.Paid,
        //            StripeSessionId = session.Id,
        //            StripePaymentIntentId = session.PaymentIntentId,
        //            TransactionId = session.PaymentIntentId
        //        };

        //        await _unitOfWork.PaymentRepository.AddAsync(payment, CancellationToken.None);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Link payment to order
        //        order.PaymentId = payment.PaymentId;
        //        await _unitOfWork.OrderRepository.UpdateAsync(order, CancellationToken.None);

        //        // 8. Update inventory
        //        foreach (var cartItem in userCart.Items)
        //        {
        //            var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
        //            if (productItem != null)
        //            {
        //                productItem.Quantity -= cartItem.RequiredQuantity;
        //                await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
        //            }
        //        }

        //        // 9. Clear cart
        //        foreach (var item in userCart.Items.ToList())
        //        {
        //            await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);
        //        }

        //        await _unitOfWork.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return ResponseHandler.Created(order);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return ResponseHandler.BadRequest<Order>(ex.Message);
        //    }
        //}
        public async Task<Response<OrderCheckOutPreviewDto>> GetCheckoutPreviewAsync(string userId, string? CouponCode = null)
        {
            try
            {
                // Get user cart
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[]
                    {
                        c => c.Items
                    }
                );

                var userCart = carts.FirstOrDefault();
                if (userCart == null || userCart.Items == null || !userCart.Items.Any())
                {
                    return ResponseHandler.BadRequest<OrderCheckOutPreviewDto>("Cart is empty");
                }

                decimal subTotal = 0;
                foreach (var item in userCart.Items)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId);
                    if (productItem != null)
                    {
                        subTotal += productItem.Price * item.RequiredQuantity;
                    }
                }

                decimal discountValue = 0;
                if (!string.IsNullOrWhiteSpace(CouponCode))
                {
                    var coupon = await _unitOfWork.CouponRepository.GetAll().FirstOrDefaultAsync(x => x.Code == CouponCode);//.GetByIdAsync(couponId.Value);
                    if (coupon != null && coupon.IsActive && DateOnly.FromDateTime(coupon.ExpireDate) >= DateOnly.FromDateTime(DateTime.Now))
                    {
                        if (subTotal >= (coupon.MinOrderAmount ?? 0))
                        {
                            discountValue = coupon.IsPercentage 
                                ? (subTotal * coupon.DiscountValue / 100)
                                : coupon.DiscountValue;
                        }
                    }
                }

                var preview = new OrderCheckOutPreviewDto
                {
                    SubTotalPrice = subTotal,
                    DiscountValue = discountValue > 0 ? discountValue : null,
                    ShippingPrice = DEFAULT_SHIPPING_PRICE,
                    TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
                    EstimatedDeliveryDate = DateTime.Now.AddDays(DEFAULT_DELIVERY_DAYS)
                };

                return ResponseHandler.Success(preview);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<OrderCheckOutPreviewDto>($"Error calculating checkout preview: {ex.Message}");
            }
        }

        public async Task<Response<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto)
        {
            try
            {
                // Validate address belongs to user
                var address = await _unitOfWork.AddressRepository.GetByIdAsync(createOrderDto.AddressId);
                if (address == null || address.ApplicationUserId != userId)
                {
                    return ResponseHandler.BadRequest<OrderDto>("Invalid address selected");
                }

                // Get user cart
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[]
                    {
                        c => c.Items
                    }
                );

                var userCart = carts.FirstOrDefault();
                if (userCart == null || userCart.Items == null || !userCart.Items.Any())
                {
                    return ResponseHandler.BadRequest<OrderDto>("Cart is empty");
                }

                // Calculate order totals
                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in userCart.Items)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
                    if (productItem == null)
                    {
                        return ResponseHandler.BadRequest<OrderDto>($"Product item {cartItem.ProductItemId} not found");
                    }

                    if (productItem.Quantity < cartItem.RequiredQuantity)
                    {
                        return ResponseHandler.BadRequest<OrderDto>($"Insufficient stock for product item {cartItem.ProductItemId}");
                    }

                    var itemTotal = productItem.Price * cartItem.RequiredQuantity;
                    subTotal += itemTotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductItemId = cartItem.ProductItemId,
                        Quantity = cartItem.RequiredQuantity,
                        Price = productItem.Price
                    });
                }

                // Apply coupon if provided
                decimal discountValue = 0;
                Coupon? coupon = null;
                if (!string.IsNullOrWhiteSpace(createOrderDto.CouponCode))
                {
                    coupon = await _unitOfWork.CouponRepository.GetAll().FirstOrDefaultAsync(x => x.Code == createOrderDto.CouponCode);//GetByIdAsync(createOrderDto.CouponId.Value);
                    if (coupon != null && coupon.IsActive && DateOnly.FromDateTime(coupon.ExpireDate) >= DateOnly.FromDateTime(DateTime.Now))
                    {
                        if (subTotal >= (coupon.MinOrderAmount ?? 0))
                        {
                            discountValue = coupon.IsPercentage
                                ? (subTotal * coupon.DiscountValue / 100)
                                : coupon.DiscountValue;

                            // No max discount amount property found in model
                        }
                    }
                }

                // Create order
                var order = new Order
                {
                    ApplicationUserId = userId,
                    SubTotalPrice = subTotal,
                    DiscountValue = discountValue > 0 ? discountValue : null,
                    ShippingPrice = DEFAULT_SHIPPING_PRICE,
                    TotalPrice = subTotal - discountValue + DEFAULT_SHIPPING_PRICE,
                    Note = createOrderDto.Note,
                    EstimatedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(DEFAULT_DELIVERY_DAYS)),
                    CouponId = coupon.CouponId,
                    Status = OrderStatus.Pending,
                    AddressId = createOrderDto.AddressId,
                    OrderItems = orderItems
                };

                await _unitOfWork.OrderRepository.AddAsync(order);

                // Create payment
                var payment = new Payment
                {
                    ApplicationUserId = userId,
                    Amount = order.TotalPrice,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    PaymentStatus = createOrderDto.PaymentMethod == PaymentMethod.CashOnDelivery 
                        ? PaymentStatus.Pending 
                        : PaymentStatus.Pending,
                    DeliveryNotes = createOrderDto.DeliveryNotes
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment, CancellationToken.None);
                await _unitOfWork.SaveChangesAsync();

                // Update order with payment ID
                order.PaymentId = payment.PaymentId;
                await _unitOfWork.OrderRepository.UpdateAsync(order, CancellationToken.None);

                // Update product quantities
                foreach (var cartItem in userCart.Items)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(cartItem.ProductItemId);
                    if (productItem != null)
                    {
                        productItem.Quantity -= cartItem.RequiredQuantity;
                        await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
                    }
                }

                // Clear cart
                foreach (var item in userCart.Items.ToList())
                {
                    await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);
                }

                await _unitOfWork.SaveChangesAsync();

                // Return order DTO
                var orderDto = await MapToOrderDto(order, address, payment, coupon);
                return ResponseHandler.Success(orderDto);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<OrderDto>($"Error creating order: {ex.Message}");
            }
        }

        public async Task<Response<List<OrderSummaryDto>>> GetUserOrdersAsync(string userId, int? status = null)
        {
            try
            {
                // Build predicate based on filters
                Expression<Func<Order, bool>> predicate = o => o.ApplicationUserId == userId;
                
                if (status.HasValue)
                {
                    var orderStatus = (OrderStatus)status.Value;
                    predicate = o => o.ApplicationUserId == userId && o.Status == orderStatus;
                }

                var orders = await _unitOfWork.OrderRepository.GetAllWithIncludeAsync(
                    predicate: predicate,
                    includes: new Expression<Func<Order, object>>[]
                    {
                        o => o.OrderItems,
                        o => o.Payment,
                        o => o.Coupon
                    }
                );

                var orderSummaries = new List<OrderSummaryDto>();

                foreach (var order in orders.OrderByDescending(o => o.CreatedDate))
                {
                    var orderItemSummaries = new List<OrderItemSummaryDto>();

                    if (order.OrderItems != null)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithIncludeAsync(
                                item.ProductItemId,
                                includes: new Expression<Func<ProductItem, object>>[] 
                                { 
                                    pi => pi.Product,
                                    pi => pi.Product.ProductImages,
                                    pi => pi.Product.Brand,
                                    pi => pi.Product.Category
                                }
                            );

                            if (productItem != null)
                            {
                                var productImage = productItem.Product?.ProductImages?.FirstOrDefault()?.URL;
                                
                                orderItemSummaries.Add(new OrderItemSummaryDto
                                {
                                    ProductItemId = item.ProductItemId,
                                    ProductName = productItem.Product?.NameEn ?? "Unknown Product",
                                    ProductImage = productImage,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    TotalPrice = item.Price * item.Quantity,
                                    Color = productItem.FabricColor?.NameEn ?? productItem.WoodColor?.NameEn,
                                    Size = null, // ProductItem doesn't have Size property
                                    Brand = productItem.Product?.Brand?.NameEn,
                                    Category = productItem.Product?.Category?.NameEn
                                });
                            }
                        }
                    }

                    orderSummaries.Add(new OrderSummaryDto
                    {
                        OrderId = order.OrderId,
                        SubTotalPrice = order.SubTotalPrice,
                        DiscountValue = order.DiscountValue,
                        ShippingPrice = order.ShippingPrice,
                        TotalPrice = order.TotalPrice,
                        EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                        Status = order.Status,
                        PaymentMethod = order.Payment?.PaymentMethod ?? PaymentMethod.CashOnDelivery,
                        PaymentStatus = order.Payment?.PaymentStatus ?? PaymentStatus.Pending,
                        CreatedDate = order.CreatedDate,
                        ItemsCount = order.OrderItems?.Sum(item => item.Quantity) ?? 0,
                        OrderItems = orderItemSummaries,
                        CouponCode = order.Coupon?.Code
                    });
                }

                return ResponseHandler.Success(orderSummaries);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<OrderSummaryDto>>($"Error retrieving orders: {ex.Message}");
            }
        }

        public async Task<Response<OrderDto>> GetOrderByIdAsync(string userId, int orderId)
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllWithIncludeAsync(
                    predicate: o => o.OrderId == orderId && o.ApplicationUserId == userId,
                    includes: new Expression<Func<Order, object>>[]
                    {
                        o => o.OrderItems,
                        o => o.DeliveryAddress,
                        o => o.Payment,
                        o => o.Coupon
                    }
                );

                var order = orders.FirstOrDefault();
                if (order == null)
                {
                    return ResponseHandler.NotFound<OrderDto>("Order not found");
                }

                var orderDto = await MapToOrderDto(order, order.DeliveryAddress, order.Payment, order.Coupon);
                return ResponseHandler.Success(orderDto);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<OrderDto>($"Error retrieving order: {ex.Message}");
            }
        }

        public async Task<Response<bool>> CancelOrderAsync(string userId, int orderId)
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllWithIncludeAsync(
                    predicate: o => o.OrderId == orderId && o.ApplicationUserId == userId,
                    includes: new Expression<Func<Order, object>>[]
                    {
                        o => o.OrderItems
                    }
                );

                var order = orders.FirstOrDefault();
                if (order == null)
                {
                    return ResponseHandler.NotFound<bool>("Order not found");
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return ResponseHandler.BadRequest<bool>("Only pending orders can be cancelled");
                }

                // Update order status
                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.OrderRepository.UpdateAsync(order, CancellationToken.None);

                // Restore product quantities
                if (order.OrderItems != null)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(orderItem.ProductItemId);
                        if (productItem != null)
                        {
                            productItem.Quantity += orderItem.Quantity;
                            await _unitOfWork.ProductItemRepository.UpdateAsync(productItem, CancellationToken.None);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"Error cancelling order: {ex.Message}");
            }
        }

        public async Task<Response<List<OrderSummaryDto>>> GetAllOrdersAsync(string userId)
        {
            try
            {
                var allOrders = await _unitOfWork.OrderRepository.GetAllWithIncludeAsync(
                    predicate: o => o.ApplicationUserId == userId, // Get orders for specific user only
                    includes: new Expression<Func<Order, object>>[]
                    {
                        o => o.OrderItems,
                        o => o.Payment,
                        o => o.Coupon,
                        o => o.ApplicationUser
                    }
                );

                var orders = allOrders.OrderByDescending(o => o.CreatedDate).ToList();
                var orderSummaries = new List<OrderSummaryDto>();

                foreach (var order in orders)
                {
                    var orderItemSummaries = new List<OrderItemSummaryDto>();

                    if (order.OrderItems != null)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithIncludeAsync(
                                item.ProductItemId,
                                includes: new Expression<Func<ProductItem, object>>[] 
                                { 
                                    pi => pi.Product,
                                    pi => pi.Product.ProductImages,
                                    pi => pi.Product.Brand,
                                    pi => pi.Product.Category
                                }
                            );

                            if (productItem != null)
                            {
                                var productImage = productItem.Product?.ProductImages?.FirstOrDefault()?.URL;
                                
                                orderItemSummaries.Add(new OrderItemSummaryDto
                                {
                                    ProductItemId = item.ProductItemId,
                                    ProductName = productItem.Product?.NameEn ?? "Unknown Product",
                                    ProductImage = productImage,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    TotalPrice = item.Price * item.Quantity,
                                    Color = productItem.FabricColor?.NameEn ?? productItem.WoodColor?.NameEn,
                                    Size = null, // ProductItem doesn't have Size property
                                    Brand = productItem.Product?.Brand?.NameEn,
                                    Category = productItem.Product?.Category?.NameEn
                                });
                            }
                        }
                    }

                    orderSummaries.Add(new OrderSummaryDto
                    {
                        OrderId = order.OrderId,
                        SubTotalPrice = order.SubTotalPrice,
                        DiscountValue = order.DiscountValue,
                        ShippingPrice = order.ShippingPrice,
                        TotalPrice = order.TotalPrice,
                        EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                        Status = order.Status,
                        PaymentMethod = order.Payment?.PaymentMethod ?? PaymentMethod.CashOnDelivery,
                        PaymentStatus = order.Payment?.PaymentStatus ?? PaymentStatus.Pending,
                        CreatedDate = order.CreatedDate,
                        ItemsCount = order.OrderItems?.Sum(item => item.Quantity) ?? 0,
                        OrderItems = orderItemSummaries,
                        CouponCode = order.Coupon?.Code
                    });
                }

                return ResponseHandler.Success(orderSummaries);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<OrderSummaryDto>>($"Error retrieving user orders: {ex.Message}");
            }
        }

        private async Task<OrderDto> MapToOrderDto(Order order, Address? address, Payment? payment, Coupon? coupon)
        {
            var orderItems = new List<OrderItemDto>();

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithIncludeAsync(
                        item.ProductItemId,
                        includes: new Expression<Func<ProductItem, object>>[] 
                        { 
                            pi => pi.Product,
                            pi => pi.Product.ProductImages,
                            pi => pi.Product.Brand,
                            pi => pi.Product.Category
                        }
                    );

                    if (productItem != null)
                    {
                        var productImage = productItem.Product?.ProductImages?.FirstOrDefault()?.URL;
                        
                        orderItems.Add(new OrderItemDto
                        {
                            OrderItemId = item.OrderItemId,
                            ProductItemId = item.ProductItemId,
                            ProductName = productItem.Product?.NameEn ?? "Unknown Product",
                            ProductImage = productImage,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            TotalPrice = item.Price * item.Quantity,
                            Color = productItem.FabricColor?.NameEn ?? productItem.WoodColor?.NameEn,
                            Size = null, // ProductItem doesn't have Size property
                            Brand = productItem.Product?.Brand?.NameEn,
                            Category = productItem.Product?.Category?.NameEn
                        });
                    }
                }
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                ApplicationUserId = order.ApplicationUserId,
                OrderItems = orderItems,
                SubTotalPrice = order.SubTotalPrice,
                DiscountValue = order.DiscountValue,
                ShippingPrice = order.ShippingPrice,
                TotalPrice = order.TotalPrice,
                Note = order.Note,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                CouponId = order.CouponId,
                CouponCode = coupon?.Code,
                Status = order.Status,
                AddressId = order.AddressId,
                DeliveryAddress = address != null ? $"{address.FullAddress}, {address.Area}, {address.FloorNumber}" : null,
                PaymentId = order.PaymentId,
                PaymentMethod = payment?.PaymentMethod ?? PaymentMethod.CashOnDelivery,
                PaymentStatus = payment?.PaymentStatus ?? PaymentStatus.Pending,
                TransactionId = payment?.TransactionId,
                CreatedDate = order.CreatedDate
            };
        }
    }
}