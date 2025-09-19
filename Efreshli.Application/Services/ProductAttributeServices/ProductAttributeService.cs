using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductAttributeServices
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductAttributeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<ProductAttributeResponseDto>> CreateAttributeAsync(CreateProductAttributeDto createDto)
        {
            if(createDto == null)
            {
                return ResponseHandler.BadRequest<ProductAttributeResponseDto>("Create DTO cannot be null.");
            }
            var attribute = new ProductAttribute
            {
                NameAr = createDto.NameAr,
                NameEn = createDto.NameEn,
                CategoryId = createDto.CategoryId,
                Category = createDto.CategoryId.HasValue ? await _unitOfWork.CategoryRepository.GetByIdAsync(createDto.CategoryId.Value) : null
            };
            await _unitOfWork.ProductAttributeRepository.AddAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
            var responseDto = new ProductAttributeResponseDto
            {
                ProductAttributeId = attribute.Id,
                ProductAttributeNameAr = attribute.NameAr,
                ProductAttributeNameEn = attribute.NameEn
            };

            return ResponseHandler.Success(responseDto);
        }

        public async Task<Response<bool>> DeleteAttributeAsync(int id)
        {
            var attribute = await _unitOfWork.ProductAttributeRepository.GetByIdAsync(id);
            if (attribute == null)
            {
                return ResponseHandler.NotFound<bool>($"Product attribute with ID {id} not found.");
            }
            await _unitOfWork.ProductAttributeRepository.RemoveAsync(attribute.Id);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(true, "Product attribute deleted successfully.");
        }

        public async Task<Response<List<ProductAttributeResponseDto>>> GetAllAttributesAsync()
        {

            var attributes = await _unitOfWork.ProductAttributeRepository.GetAllAsync();
            var responseDtos = attributes
                .GroupBy(attr => new { attr.NameAr, attr.NameEn }) // تجميع حسب الاسم العربي والإنجليزي
                .Select(group => group.First())
                .Select(attribute => new ProductAttributeResponseDto
            {
                ProductAttributeId = attribute.Id,
                ProductAttributeNameAr = attribute.NameAr,
                ProductAttributeNameEn = attribute.NameEn
            })
                .OrderBy(dto => dto.ProductAttributeNameEn)
                . ToList();
            return ResponseHandler.Success(responseDtos);
        }
        public async Task<Response<List<LocalizedProductAttributeResponseDto>>> GetAllAttributesByIdAsync(int? CategoryId)
        {
            if (CategoryId == null)
            {
                var allAttributes = await _unitOfWork.ProductAttributeRepository.GetAllAsync();
                var allResponseDtos = allAttributes.Select(attribute => new LocalizedProductAttributeResponseDto
                {
                    ProductAttributeId = attribute.Id,
                    ProductAttributeName = attribute.GetLocalized(attribute.NameAr, attribute.NameEn)
                }).ToList();
                return ResponseHandler.Success(allResponseDtos);
            }
            var attributes = await _unitOfWork.ProductAttributeRepository.GetWhereAsync(i=>i.CategoryId==CategoryId);
            var responseDtos = attributes.Select(attribute => new LocalizedProductAttributeResponseDto
            {
                ProductAttributeId = attribute.Id,
                ProductAttributeName = attribute.GetLocalized(attribute.NameAr,attribute.NameEn)
            }).ToList();
            return ResponseHandler.Success(responseDtos);
        }

        public async Task<Response<ProductAttributeResponseDto>> GetAttributeByIdAsync(int id)
        {
            var attribute = await _unitOfWork.ProductAttributeRepository.GetByIdAsync(id);
            if (attribute == null)
            {
                return ResponseHandler.NotFound<ProductAttributeResponseDto>($"Product attribute with ID {id} not found.");
            }
            var responseDto = new ProductAttributeResponseDto
            {
                ProductAttributeId = attribute.Id,
                ProductAttributeNameAr = attribute.NameAr,
                ProductAttributeNameEn = attribute.NameEn
            };
            return ResponseHandler.Success(responseDto);
        }

        public async Task<Response<ProductAttributeResponseDto>> UpdateAttributeAsync(int id, UpdateProductAttributeDto updateDto)
        {
            if (updateDto == null)
            {
                return ResponseHandler.BadRequest<ProductAttributeResponseDto>("Update DTO cannot be null.");
            }
            var attribute = await _unitOfWork.ProductAttributeRepository.GetByIdAsync(id);
            if (attribute == null)
            {
                return ResponseHandler.NotFound<ProductAttributeResponseDto>("Product attribute not found.");
            }
            if (attribute.NameAr == null)
            {
                updateDto.NameAr = attribute.NameAr;
            }
            if (attribute.NameEn == null)
            {
                updateDto.NameEn = attribute.NameEn;
            }
            attribute.NameAr = updateDto.NameAr;
            attribute.NameEn = updateDto.NameEn;
            await _unitOfWork.ProductAttributeRepository.UpdateAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
            var responseDto = new ProductAttributeResponseDto
            {
                ProductAttributeId = attribute.Id,
                ProductAttributeNameAr = attribute.NameAr,
                ProductAttributeNameEn = attribute.NameEn
            };
            return ResponseHandler.Success(responseDto);
        }
    }
}
