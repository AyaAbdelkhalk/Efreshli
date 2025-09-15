using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.FilterServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IFilterService _filterService;

        public FilterController(IFilterService filterService)
        {
            _filterService = filterService;
        }
        [HttpGet("brands/{categoryId}")]
        public async Task<IActionResult> GetBrandsByCategoryId(int categoryId)
        {
            var brands = await _filterService.GetBrandsByCategoryId(categoryId);
            return this.CreateResponse(brands);
        }
        [HttpGet("Fabriccolors/{categoryId}")]
        public async Task<IActionResult> GetFabricColorsByCategoryId(int categoryId)
        {
            var colors = await _filterService.GetFabricColorsByCategoryId(categoryId);
            return this.CreateResponse(colors);
        }
        [HttpGet("Woodcolors/{categoryId}")]
        public async Task<IActionResult> GetWoodColorsByCategoryId(int categoryId)
        {
            var colors = await _filterService.GetWoodColorsByCategoryId(categoryId);
            return this.CreateResponse(colors);
        }
        //filter
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts([FromQuery] int? categoryId, [FromQuery] List<int>? brandIds, [FromQuery] int? fabricColorIds, [FromQuery] int? woodColorIds, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 24)
        {
            var filteredProducts = await _filterService.GetFilteredProductsAsync(categoryId, brandIds, fabricColorIds, woodColorIds, minPrice, maxPrice, pageNumber, pageSize);
            return this.CreateResponse(filteredProducts);
        }

    }
}
