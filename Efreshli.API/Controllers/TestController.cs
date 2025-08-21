using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Application.Services.File;
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

    }
}
