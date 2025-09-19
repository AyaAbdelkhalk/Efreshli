using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Classes;

namespace Efreshli.Application.Services.HomeServices
{
    public interface IHomeService
    {
        public Task<Response<PaginatedResult<FilteredProductsDto>>> SearchProducts(string keyword, int pageNumber = 1, int pageSize = 24);
        public Task<Response<PaginatedResult<FilteredProductsDto>>> GetBrandProducts(int BrandId);
        //public Task<Response<PaginatedResult<FilteredProductsDto>>> GetColorProducts(int ColorId, int pageNumber = 1, int pageSize = 30);


    }
}
