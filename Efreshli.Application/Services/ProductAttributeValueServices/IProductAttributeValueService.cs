using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductAttributeValueServices
{
    public interface IProductAttributeValueService
    {
        Task<Response<List<ProductAttributeValueResponseDto>>> GetAllProductAttributeValuesAsync();
        Task<Response<List<ProductAttributeValueResponseDto>>> GetProductSpecificationAsync(int productId);

        Task<Response<ProductAttributeValueResponseDto>> GetProductAttributeValueByIdAsync(int id);
        Task<Response<ProductAttributeValueResponseDto>> CreateProductAttributeValueAsync(CreateProductAttributeValueDto createDto);
        Task<Response<ProductAttributeValueResponseDto>> UpdateProductAttributeValueAsync(int id, UpdateProductAttributeValueDto updateDto);
        Task<Response<ProductAttributeValueResponseDto>> DeleteProductAttributeValueAsync(int id);
    }
}
