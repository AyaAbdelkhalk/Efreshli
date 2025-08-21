using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Resources;
using Efreshli.Application.Services.CouponServices;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.Threading.Tasks;

namespace Efreshli.MVC.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public CouponController(ICouponService couponService,IStringLocalizer<SharedResources> stringLocalizer)
        {
            _couponService = couponService;
            this._stringLocalizer = stringLocalizer;
        }
        // GET: CouponController
        public async Task<ActionResult> Index()
        {
            var coupons = await _couponService.GetAllCouponsAsync();

            return View(coupons);
        }

        // GET: CouponController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            
            var coupoun = await _couponService.GetCouponByIdAsync(id);
            return View(coupoun);
        }

        // GET: CouponController/Create
        public ActionResult Create()
        {
            return View(new AddCouponDTO());
        }

        // POST: CouponController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddCouponDTO addCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(addCouponDTO);
                }         
                var result = await _couponService.CreateCouponAsync(addCouponDTO);
                if (result != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If creation failed but no exception was thrown
                    ModelState.AddModelError("", _stringLocalizer[SharedResourcesKeys.Error.ServerError]);
                    return View(addCouponDTO);
                }
            }
            catch (Exception ex)
            {
                // Add a proper error message to the ModelState
                ModelState.AddModelError("", $"{ _stringLocalizer[SharedResourcesKeys.Error.GeneralError]} ");
                return View(addCouponDTO);
            }
        }

        // GET: CouponController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
           var couponDb=await _couponService.GetCouponByIdAsync(id);
            if(couponDb == null)
            {
                return NotFound();
            }
            var copounDto = couponDb.Adapt<UpdateCouponDTO>();
            return View(copounDto);
        }

        // POST: CouponController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateCouponDTO updateCouponDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(updateCouponDTO);
                }

                 await _couponService.UpdateCouponAsync(updateCouponDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Add a proper error message to the ModelState
                ModelState.AddModelError("", $"{_stringLocalizer[SharedResourcesKeys.Error.GeneralError]} ");
                return View(updateCouponDTO);
            }
        }

        // GET: Coupon/Delete/5 - Shows the confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupon/Delete/5 - Handles the actual deletion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection) // The extra parameter differentiates it
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            var response = await _couponService.DeleteCouponAsync(id);
            if (!response.Succeeded)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Failed to delete coupon.");
                return View(coupon);
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _couponService.ActivateCouponAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _couponService.DeactivateCouponAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Checkout()
        {
            var Currency = "usd";
            var surl = "http://localhost:5136/Coupon/Details/1";
            var cancelurl = "http://localhost:5136/Coupon";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = surl,
                CancelUrl = cancelurl,
                Mode = "payment",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
    {
        new Stripe.Checkout.SessionLineItemOptions
        {
            PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
            {
                Currency = Currency,
                UnitAmount = 5000, // amount in cents, e.g. 5000 = $50.00
                ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                {
                    Name = "Final Payment"
                }
            },
            Quantity = 1
        }
    }
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
    }
}
