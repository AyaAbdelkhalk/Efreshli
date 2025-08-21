using Efreshli.Application.Services.ProductAttributeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeDTOs;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAttributeController : ControllerBase
    {
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeController(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAttributes()
        {
            var response = await _productAttributeService.GetAllAttributesAsync();
            return this.CreateResponse(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAttribute([FromBody] CreateProductAttributeDto createDto)
        {
            var response = await _productAttributeService.CreateAttributeAsync(createDto);
            return this.CreateResponse(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttributeById(int id)
        {
            var response = await _productAttributeService.GetAttributeByIdAsync(id);
            return this.CreateResponse(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttribute(int id, [FromBody] UpdateProductAttributeDto updateDto)
        {
            var response = await _productAttributeService.UpdateAttributeAsync(id, updateDto);
            return this.CreateResponse(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute(int id)
        {
            var response = await _productAttributeService.DeleteAttributeAsync(id);
            return this.CreateResponse(response);
        }
    }
}
