using Efreshli.Application.DTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.FilterServices;
using Efreshli.Domain.Enums;
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
        public async Task<IActionResult> FilterProducts(
            string? keyword=null,
            int pageNumber = 1,
            int pageSize = 24,
            ProductSortBy sortBy = ProductSortBy.Recommended,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? categoryId = null,
            [FromQuery] List<int>? brandIds = null,
            int? fabricColorId = null,
            int? woodColorId = null)
        {
            var filterRequest = new ProductFilterRequest
            {
                Keyword = keyword,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CategoryId = categoryId,
                BrandIds = brandIds ?? new List<int>(),
                FabricColorId = fabricColorId,
                WoodColorId = woodColorId
            };
            var filteredProducts = await _filterService.GetFilteredProductsAsync(filterRequest);
            return this.CreateResponse(filteredProducts);
        }
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters(int? categoryId = null)
        {
            try
            {
                var result = new
                {
                    Brands = await _filterService.GetBrandsByCategoryId(categoryId ?? 0),
                    FabricColors = categoryId.HasValue ?
                        await _filterService.GetFabricColorsByCategoryId(categoryId.Value) :
                        null,
                    WoodColors = categoryId.HasValue ?
                        await _filterService.GetWoodColorsByCategoryId(categoryId.Value) :
                        null,
                    SortOptions = new[]
                    {
                        new { Value = 0, NameAr = "الموصى بها", NameEn = "Recommended" },
                        new { Value = 1, NameAr = "الأحدث", NameEn = "Latest Products" },
                        new { Value = 2, NameAr = "السعر من الأعلى للأقل", NameEn = "Price High To Low" },
                        new { Value = 3, NameAr = "السعر من الأقل للأعلى", NameEn = "Price Low To High" },
                        new { Value = 4, NameAr = "الأكثر مبيعاً", NameEn = "Best Selling" }
                    }
                };

                return this.CreateResponse(ResponseHandler.Success(result));
            }
            catch (Exception ex)
            {
                return this.CreateResponse(ResponseHandler.BadRequest<object>($"Error getting filters: {ex.Message}"));
            }
        }
    }
}
