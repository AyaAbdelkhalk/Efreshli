using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Services.CouponServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Efreshli.MVC.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
           _couponService = couponService;
        }
        // GET: CouponController
        public async Task<ActionResult> Index()
        {
           var coupons=await _couponService.GetAllCouponsAsync();

            return View(coupons);
        }

        // GET: CouponController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var coupoun =await _couponService.GetCouponByIdAsync(id);
            return View(coupoun);
        }

        // GET: CouponController/Create
        public ActionResult Create(AddCouponDTO addCouponRequest)
        {
           
            return View(addCouponRequest);
        }

        // POST: CouponController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CouponController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CouponController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
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



        //[HttpPost]
        //public async Task<IActionResult> Activate(int id)
        //{
        //    var result = await _couponService.ActivateCouponAsync(id);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }
        //    return RedirectToAction(nameof(Details), new { id });
        //}

        //[HttpPost]
        //public async Task<IActionResult> Deactivate(int id)
        //{
        //    var result = await _couponService.DeactivateCouponAsync(id);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }
        //    return RedirectToAction(nameof(Details), new { id });
        //}


    }
}
