using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.FilterServices;
using Efreshli.Application.Services.HomeServices;
using Efreshli.Application.Services.ProductServices;
using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHomeService _homeService;
        private readonly IFilterService _filterService;

        public CollectionsController(IProductService productService, IHomeService homeService, IFilterService filterService)
        {
            _productService = productService;
            _homeService = homeService;
            _filterService = filterService;
        }
        [HttpGet("newProducts")]
        public async Task<IActionResult> GetNewArrivals(int pageNumber = 1, int pageSize = 24)
        {
            var result = await _productService.GetNewArrivals(pageNumber, pageSize);
            return this.CreateResponse(result);

        }
        [HttpGet("{keyword}")]
        public async Task<IActionResult> GetCollections(string? keyword=null,
            ProductSortBy sortBy = ProductSortBy.Recommended,
            int pageNumber = 1, int pageSize = 24)
        {
            var rs = await _homeService.SearchProducts(keyword, pageNumber, pageSize);
            if (rs.Succeeded)
            {
                return this.CreateResponse(rs);
            }
            return BadRequest(rs);
        }
        //[HttpGet("wall-mounted-decor")]
        //public async Task<IActionResult> GetWallMountedDecor(int pageNumber = 1, int pageSize = 24)
        //{
        //    var rs = await _homeService.SearchProducts("wall decor", pageNumber, pageSize);
        //    if (rs.Succeeded)
        //    {
        //        return this.CreateResponse(rs);
        //    }
        //    return BadRequest(rs);
        //}

        [HttpGet("tables")]
        public async Task<IActionResult> GetTableProducts(int pageNumber = 1, int pageSize = 24)
        {
            var rs = await _homeService.SearchProducts("table", pageNumber, pageSize);
            if (rs.Succeeded)
            {
                return this.CreateResponse(rs);
            }
            return BadRequest(rs);
        }

    }
}
