using Efreshli.Application.Services.OrderServices;
using Efreshli.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Efreshli.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        public StripeWebhookController(IConfiguration config, IUnitOfWork unitOfWork, IOrderService orderService)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
        
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];


            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, signature, _config["Stripe:WebhookSecret"], tolerance: 300,throwOnApiVersionMismatch: false);
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session == null)
                        return BadRequest();

                    // Idempotency: check if this session already processed
                    var existingPayment = await _unitOfWork.PaymentRepository.GetAll()
                        .FirstOrDefaultAsync(p => p.StripeSessionId == session.Id);

                    if (existingPayment != null)
                    {
                        // Already processed — respond 200
                        return Ok();
                    }

                    // Create order from the session metadata (and perform the DB updates)
                    await _orderService.CreateOrderFromStripeSessionAsync(session);

                    return Ok();
                }

                // handle other event types if needed
                return Ok();
            }
            catch (StripeException se)
            {
                // log and return 400
                return BadRequest(se.Message);
            }
        }
    }

}
