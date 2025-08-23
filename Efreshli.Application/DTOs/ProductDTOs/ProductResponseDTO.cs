using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs
{
    public class ProductResponseDTO
    {
        public int ProductId { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string? DimensionsOrSize { get; set; }
        public string SKU { get; set; }
        public string? CategoryNameAr { get; set; }
        public string? CategoryNameEn { get; set; }
        public string BrandNameAr { get; set; }
        public string BrandNameEn { get; set; }
        public List<ProductColorDto>? ProductItemColors { get; set; } = new List<ProductColorDto>();
        public List<ProductItemResponseDto>? ProductItems { get; set; } = new List<ProductItemResponseDto>();
        public List<string>? ProductImageUrls { get; set; } = new List<string>();
        public List<ProductAttributeValueResponseDto>? AttributeValues { get; set; } = new List<ProductAttributeValueResponseDto>();

        public bool IsWishlisted { get; set; } = false;


    }
}
