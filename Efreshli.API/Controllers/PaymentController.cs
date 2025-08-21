using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;
using System.Threading.Tasks;
using Order = Efreshli.Domain.Models.Order;

namespace Efreshli.API.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;
       private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IConfiguration config,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }
        //[Authorize("user")]
        //[Route("che")]
        [HttpPost("Checkout")]
        public IActionResult Checkout(string? couponCode="")
        {
            //        var Currency = "usd";
            //        var surl = "http://localhost:5136/Coupon/Details/1";
            //        var cancelurl = "http://localhost:5136/Coupon";

            //        var options = new Stripe.Checkout.SessionCreateOptions
            //        {
            //            SuccessUrl = surl,
            //            CancelUrl = cancelurl,
            //            Mode = "payment",
            //            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            //{
            //    new Stripe.Checkout.SessionLineItemOptions
            //    {
            //        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
            //        {
            //            Currency = Currency,
            //            UnitAmount = 5000, // amount in cents, e.g. 5000 = $50.00
            //            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
            //            {
            //                Name = "Final Payment"
            //            }
            //        },
            //        Quantity = 1
            //    }
            //}
            //        };

            //        var service = new Stripe.Checkout.SessionService();
            //        Stripe.Checkout.Session session = service.Create(options);

            //        Response.Headers.Add("Location", session.Url);
            //        return Ok(new { url = session.Url });


            var amountToPay = 20;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var options = new SessionCreateOptions
            {
                SuccessUrl = "http://localhost:5136/Coupon/Details/1",
                CancelUrl = "http://www.google.com",
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(amountToPay * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Final Payment"
                            }
                        },
                        Quantity = 1
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    { "userId",userId  },
                    { "amount",amountToPay.ToString() }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { url = session.Url });


        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                var endpointSecret = _config["Stripe:WebhookSecret"];
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                );

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    var userId = session.Metadata["userId"];
                    var amount = decimal.Parse(session.Metadata["amount"]);

                    // ✅ Create Payment
                    var payment = new Payment
                    {
                        TransactionId = session.Id,
                        Amount = amount,
                        PaymentStatus = PaymentStatus.Paid,
                        ApplicationUserId = userId
                    };
                    var paymentFrpmDB= await   _unitOfWork.PaymentRepository.AddAsync(payment);

                    // ✅ Create Order
                    var order = new Order
                    {
                        ApplicationUserId = userId,
                        TotalPrice = amount,
                        Status = OrderStatus.Pending,
                       PaymentId=paymentFrpmDB.PaymentId
                    };
                   await _unitOfWork.OrderRepository.AddAsync(order);
                }
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }

            return Ok();
        }
    }
}
