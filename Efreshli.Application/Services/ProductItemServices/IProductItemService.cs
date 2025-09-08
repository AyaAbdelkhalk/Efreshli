using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductItemServices
{
    public interface IProductItemService
    {
        Task<Response<ProductItemResponseDto>> CreateProductItemAsync(int PrdId,CreateProductItemDto createProductItemDto);
        Task<Response<List<ProductItemResponseDto>>> GetAllProductItemsAsync(int productId);
        Task<Response<List<ProductItemDetailsDto>>> GetProductItemsDetailsForAdminAsync(int productId);
        //delete product item
        Task<Response<bool>> DeleteProductItemAsync(int productItemId);
        Task<Response<List<string>>> GetProductItemColorsUrlsAsync(int productItemId);
        Task<Response<List<ProductDetailsColorDto>>> GetProductItemColorsDetailsAsync(int productItemId);
        Task<Dictionary<int, List<string>>> GetProductsColorsUrlsDictionaryAsync(List<int> productIds);


    }
}
