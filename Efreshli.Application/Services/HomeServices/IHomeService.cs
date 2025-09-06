using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Classes;

namespace Efreshli.Application.Services.HomeServices
{
    public interface IHomeService
    {
        public Task<Response<PaginatedResult<FilteredProductsDto>>> SearchProducts(string keyword, int pageNumber = 1, int pageSize = 24);
        public Task<Response<PaginatedResult<FilteredProductsDto>>> FilterByBrand(int pageNumber = 1, int pageSize = 30);
        public Task<Response<PaginatedResult<FilteredProductsDto>>> FilterByColor(int pageNumber = 1, int pageSize = 30);


    }
}
