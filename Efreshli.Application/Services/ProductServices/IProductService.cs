using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductServices
{
    public interface IProductService
    {
        Task<Response<ProductResponseDTO>> CreateProductAsync(CreateProductDto createProductDto);
        Task<Response<List<ProductResponseDTO>>> GetAllProductsAsync();
        Task<Response<List<MainProductsDto>>> GetMainProductsAsync(int? CategoryId);
        Task<Response<ProductDetailsDto>> GetProductDetailsForAdminAsync(int productId);
        Task<Response<GetWishlistItemDto>> GetWishlistItemsForUserAsync(int productId);


        //Task<int> CreateProductItemAsync(CreateProductItemDto createProductItemDto);
        //Task<int> CreateProductAttributeAsync(CreateProductAttributeDto createProductAttributeDto);
        //Task<int> CreateProductAttributeValueAsync(CreateProductAttributeValueDto createProductAttributeValueDto);
        //Task<bool> UpdateProductAsync(UpdateProductDto updateProductDto);
        //Task<bool> UpdateProductItemAsync(UpdateProductItemDto updateProductItemDto);
        //Task<bool> UpdateProductAttributeAsync(UpdateProductAttributeDto updateProductAttributeDto);
        //Task<bool> UpdateProductAttributeValueAsync(UpdateProductAttributeValueDto updateProductAttributeValueDto);
        Task<bool> DeleteProductAsync(int productId);
        //Task<bool> DeleteProductItemAsync(int productItemId);
        //Task<bool> DeleteProductAttributeAsync(int productAttributeId);
        //Task<bool> DeleteProductAttributeValueAsync(int productAttributeValueId);
    }
}
