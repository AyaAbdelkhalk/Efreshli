using Efreshli.Application.DTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Classes;

namespace Efreshli.Application.Services.FilterServices
{
    public interface IFilterService
    {
        Task<Response<PaginatedResult<FilteredProductsDto>>> GetFilteredProductsAsync(
            int? categoryId,
            List<int>? brandIds,
            int? fabricColorId,
            int? woodColorId,
            decimal? fromPrice,
            decimal? toPrice,
            int pageNumber = 1,
            int pageSize = 24);

        Task<Response<List<DropDownDto>>> GetBrandsByCategoryId(int categoryId);
        Task<Response<List<ColorsDropDownDto>>> GetFabricColorsByCategoryId(int categoryId);
        Task<Response<List<ColorsDropDownDto>>> GetWoodColorsByCategoryId(int categoryId);
    }
}
