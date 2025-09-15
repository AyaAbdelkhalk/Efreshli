using Efreshli.Application.DTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Classes;

namespace Efreshli.Application.Services.FilterServices
{
    public interface IFilterService
    {
        Task<Response<PaginatedResult<FilteredProductsDto>>> GetFilteredProductsAsync(ProductFilterRequest request);

        Task<Response<List<DropDownDto>>> GetBrandsByCategoryId(int categoryId);
        Task<Response<List<ColorsDropDownDto>>> GetFabricColorsByCategoryId(int categoryId);
        Task<Response<List<ColorsDropDownDto>>> GetWoodColorsByCategoryId(int categoryId);
    }
}
