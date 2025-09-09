using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Services.ProductServices;
using Efreshli.Application.Helper.ResultPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            var response = await _productService.CreateProductAsync(createProductDto);
            return this.CreateResponse(response);
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return this.CreateResponse(response);
        }
        [HttpGet("GetMainProducts/{categoryId:int?}")]
          
        public async Task<IActionResult> GetMainProducts(int? categoryId= null)
        {
            var response = await _productService.GetMainProductsAsync(categoryId);
            return this.CreateResponse(response);
        }
        [HttpGet("GetMainProducts")]
        public async Task<IActionResult> GetMainProducts()
        {
            var response = await _productService.GetMainProductsAsync(null);
            return this.CreateResponse(response);
        }
        [HttpGet("GetProductById/{productId:int}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var response = await _productService.GetProductDetailsAsync(productId);
            return this.CreateResponse(response);
        }
    }
}
