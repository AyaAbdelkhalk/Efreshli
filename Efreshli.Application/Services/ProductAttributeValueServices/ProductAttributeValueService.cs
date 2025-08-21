using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductAttributeValueServices
{
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductAttributeValueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<List<ProductAttributeValueResponseDto>>> GetAllProductAttributeValuesAsync()
        {
            var productAttributeValues = await _unitOfWork.ProductAttributeValueRepository.GetAllWithIncludeAsync(
                predicate: null,
                includes: new System.Linq.Expressions.Expression<Func<Domain.Models.ProductAttributeValue, object>>[]
                {
                    pav => pav.ProductAttribute
                }
                );
            var response = productAttributeValues.Select(pav => new ProductAttributeValueResponseDto
            {
                ProductAttributeValueId = pav.Id,
                ProductAttributeNameAr = pav.ProductAttribute.NameAr,
                ProductAttributeNameEn = pav.ProductAttribute.NameEn,
                Value = pav.Value
            }).ToList();
            return new Response<List<ProductAttributeValueResponseDto>>(response);
        }
        public async Task<Response<ProductAttributeValueResponseDto>> GetProductAttributeValueByIdAsync(int id)
        {
            var productAttributeValue = await _unitOfWork.ProductAttributeValueRepository.GetByIdWithIncludeAsync(id,
                includes: new System.Linq.Expressions.Expression<Func<Domain.Models.ProductAttributeValue, object>>[]
                {
                    pav => pav.ProductAttribute
                }
                );
            if (productAttributeValue == null)
            {
                return new Response<ProductAttributeValueResponseDto>("Product attribute value not found");
            }
            var response = new ProductAttributeValueResponseDto
            {
                ProductAttributeValueId = productAttributeValue.Id,
                ProductAttributeNameAr = productAttributeValue.ProductAttribute.NameAr,
                ProductAttributeNameEn = productAttributeValue.ProductAttribute.NameEn,
                Value = productAttributeValue.Value
            };
            return new Response<ProductAttributeValueResponseDto>(response);
        }
        public async Task<Response<ProductAttributeValueResponseDto>> CreateProductAttributeValueAsync(CreateProductAttributeValueDto createDto)
        {
            var productAttributeValue = new Domain.Models.ProductAttributeValue
            {
                ProductId = createDto.ProductId,
                ProductAttributeId = createDto.ProductAttributeId,
                Value = createDto.Value
            };
            await _unitOfWork.ProductAttributeValueRepository.AddAsync(productAttributeValue);
            await _unitOfWork.SaveChangesAsync();
            //var response = new ProductAttributeValueResponseDto
            //{
            //    ProductAttributeValueId = productAttributeValue.Id,
            //    ProductAttributeNameAr = productAttributeValue.ProductAttribute.NameAr,
            //    ProductAttributeNameEn = productAttributeValue.ProductAttribute.NameEn,
            //    Value = productAttributeValue.Value
            //};
            //return new Response<ProductAttributeValueResponseDto>(response);
            var res = await GetProductAttributeValueByIdAsync(productAttributeValue.Id);
            return ResponseHandler.Created<ProductAttributeValueResponseDto>(res.Data);
        }
        public async Task<Response<ProductAttributeValueResponseDto>> UpdateProductAttributeValueAsync(int id, UpdateProductAttributeValueDto updateDto)
        {
            var productAttributeValue = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(id);
            if (productAttributeValue == null)
            {
                return new Response<ProductAttributeValueResponseDto>("Product attribute value not found");
            }
            //productAttributeValue.ProductAttributeId = updateDto.ProductAttributeId;
            productAttributeValue.Value = updateDto.Value;
            await _unitOfWork.ProductAttributeValueRepository.UpdateAsync(productAttributeValue);
            await _unitOfWork.SaveChangesAsync();
            productAttributeValue = await _unitOfWork.ProductAttributeValueRepository.GetByIdWithIncludeAsync(id,
                includes: new System.Linq.Expressions.Expression<Func<Domain.Models.ProductAttributeValue, object>>[]
                {
                    pav => pav.ProductAttribute
                }
                );

            var response = new ProductAttributeValueResponseDto
            {
                ProductAttributeValueId = productAttributeValue.Id,
                ProductAttributeNameAr = productAttributeValue.ProductAttribute.NameAr,
                ProductAttributeNameEn = productAttributeValue.ProductAttribute.NameEn,
                Value = productAttributeValue.Value
            };
            return ResponseHandler.Updated<ProductAttributeValueResponseDto>(response);
        }

        public async Task<Response<ProductAttributeValueResponseDto>> DeleteProductAttributeValueAsync(int id)
        {
            var productAttributeValue = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(id);
            if (productAttributeValue == null)
            {
                return ResponseHandler.NotFound<ProductAttributeValueResponseDto>("Product attribute value not found");
            }
            await _unitOfWork.ProductAttributeValueRepository.RemoveAsync(productAttributeValue.Id);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Deleted<ProductAttributeValueResponseDto>();
        }

        public async Task<Response<List<ProductAttributeValueResponseDto>>> GetProductSpecificationAsync(int productId)
        {
            var productAttributeValues = await _unitOfWork.ProductAttributeValueRepository.GetAllWithIncludeAsync(
                pav => pav.ProductId == productId,
                includes: new System.Linq.Expressions.Expression<Func<Domain.Models.ProductAttributeValue, object>>[]
                {
                    pav => pav.ProductAttribute
                }
            );
            if (productAttributeValues == null || !productAttributeValues.Any())
            {
                return ResponseHandler.NotFound<List<ProductAttributeValueResponseDto>>("Product specifications not found");
            }

            var response = productAttributeValues.Select(av => new ProductAttributeValueResponseDto
            {
                ProductAttributeValueId = av.Id,
                ProductAttributeNameAr = av.ProductAttribute.NameAr,
                ProductAttributeNameEn = av.ProductAttribute.NameEn,
                Value = av.Value
            }).ToList();

            return ResponseHandler.Success(response);
        }
    }
}
