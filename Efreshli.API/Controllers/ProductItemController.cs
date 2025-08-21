using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.ProductItemServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemService _productItemService;

        public ProductItemController(IProductItemService productItemService)
        {
            _productItemService = productItemService;
        }
        [HttpPost("{prdId}")]
        public async Task<IActionResult> CreateProductItem(int prdId, [FromForm] CreateProductItemDto createProductItemDto)
        {
            var response = await _productItemService.CreateProductItemAsync(prdId, createProductItemDto);
            return this.CreateResponse(response);

        }
    }
}
