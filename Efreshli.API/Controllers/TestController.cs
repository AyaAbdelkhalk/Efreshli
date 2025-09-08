using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Application.Services.File;
using Efreshli.Infrastructure.Data.Seeders;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Efreshli.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ICouponService _couponService;
        private readonly IValidator<AddCouponDTO> _validator;

        public TestController(IImageService imageService, ICouponService couponService)
        {
            this._imageService = imageService;
            _couponService = couponService;
        }



        //[HttpPost("uploadImage")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    var s = await _imageService.UploadImageAsync(file, ImageReferenceType.Category, 4);

        //    return Ok(s);
        //}

        [HttpPost("addCoupon")]
        public async Task<IActionResult> AddCoupon([FromBody] AddCouponDTO couponDTO)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var added = await _couponService.CreateCouponAsync(couponDTO);
            return Ok(added);
        }

        //delete
        [HttpDelete("deleteCoupon")]
        public async Task<IActionResult> DeleteCoupon([Required] int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return this.CreateResponse(result);
        }
        [HttpGet("vlidateCoupon")]
        public async Task<IActionResult> ValidateCoupon(
    [Required] string couponCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _couponService.ValidateCouponAsync(couponCode, userId);
                return this.CreateResponse(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while validating the coupon");
            }
        }

        /// <summary>
        /// Seed test data for order testing
        /// </summary>
        [HttpPost("seed-test-data")]
        public async Task<IActionResult> SeedTestData()
        {
            try
            {
                await TestDataSeeder.SeedTestDataAsync(HttpContext.RequestServices);
                return Ok(new { message = "Test data seeded successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error seeding test data: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get test user info for order testing
        /// </summary>
        [HttpGet("test-user-info")]
        public IActionResult GetTestUserInfo()
        {
            return Ok(new
            {
                testUser = new
                {
                    email = "testuser@example.com",
                    password = "TestUser@123",
                    username = "testuser"
                },
                availableCoupons = new object[]
                {
                    new { code = "SAVE10", discount = "10%", minOrder = 1000 },
                    new { code = "FLAT500", discount = "500 EGP", minOrder = 5000 },
                    new { code = "EXPIRED", discount = "20%", status = "Expired (for testing)" }
                },
                orderTestingSteps = new string[]
                {
                    "1. Use POST /api/test/seed-test-data to create test data",
                    "2. Login with test user credentials",
                    "3. Use GET /api/order/checkout-preview to see cart summary",
                    "4. Use GET /api/order/checkout-preview?couponId=1 to test with coupon",
                    "5. Use POST /api/order/create with AddressId and optional CouponId",
                    "6. Use GET /api/order/my-orders to see created orders"
                }
            });
        }

    }
}
