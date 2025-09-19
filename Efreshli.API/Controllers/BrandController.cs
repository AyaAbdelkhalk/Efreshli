using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.BrandsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandsService _brandsService;
        public BrandController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }
        [HttpGet("AllBrands")]
        public async Task<IActionResult> GetAllBrands(string? search = null, int page = 1, int pageSize = 10)
        {
            var brands = await _brandsService.GetAllBrandsAsync(search, page, pageSize);
            return Ok(brands);
        }
        [HttpGet("Brands")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandsService.GetAllBrandsForUserAsync();
            return this.CreateResponse(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandsService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return Ok(brand);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] CreateBrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdBrand = await _brandsService.CreateBrandAsync(brandDto);
            return Ok(new
            {
                message = "Brand created successfully",
                brand = createdBrand
            });
         }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedBrand = await _brandsService.UpdateBrandAsync(id, brandDto);
            if (updatedBrand == null)
            {
                return NotFound();
            }
            return Ok(updatedBrand);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await _brandsService.DeleteBrandAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
