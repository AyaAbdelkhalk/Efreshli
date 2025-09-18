using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductAttributeServices
{
    public interface IProductAttributeService
    {
        Task<Response<List<ProductAttributeResponseDto>>> GetAllAttributesAsync();
        Task<Response<List<LocalizedProductAttributeResponseDto>>> GetAllAttributesByIdAsync(int? CategoryId);

        Task<Response<ProductAttributeResponseDto>> GetAttributeByIdAsync(int id);
        Task<Response<ProductAttributeResponseDto>> CreateAttributeAsync(CreateProductAttributeDto createDto);

        Task<Response<ProductAttributeResponseDto>> UpdateAttributeAsync(int id, UpdateProductAttributeDto updateDto);
        Task<Response<bool>> DeleteAttributeAsync(int id);
    }
}
