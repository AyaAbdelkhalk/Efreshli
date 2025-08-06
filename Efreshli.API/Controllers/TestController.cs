using Azure.Core;
using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Application.Services.File;
using Efreshli.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ICouponService _couponService;
        private readonly IValidator<AddCouponDTO> _validator;

        public TestController(IImageService imageService, ICouponService couponService , IValidator<AddCouponDTO> validator)
        {
            this._imageService = imageService;
            _couponService = couponService;
           _validator = validator;
        }



        //[HttpPost("uploadImage")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    var s = await _imageService.UploadImageAsync(file, ImageReferenceType.Category, 4);

        //    return Ok(s);
        //}

        [HttpPost("addCoupon")]
        public async  Task<IActionResult> AddCoupon([FromBody]AddCouponDTO couponDTO)
        {
             var validationResult = await _validator.ValidateAsync(couponDTO);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var added= await _couponService.CreateCouponAsync(couponDTO);
            return Ok(added);
        }

        //delete
        [HttpDelete("deleteCoupon")]
        public async Task<IActionResult> DeleteCoupon([Required] int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return this.CreateResponse(result);  
        }

    }
}