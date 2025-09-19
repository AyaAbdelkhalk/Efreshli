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
    public class LocalizedProductDetailsDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Brand { get; set; }
        public int? BrandId { get; set; }
        public string? DimensionsOrSize { get; set; }
        public string? SKU { get; set; }
        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        public string? Model_3D { get; set; }
        public decimal MainPrice { get; set; }
        public decimal? MainFinalPrice { get; set; }
        public decimal MainDiscount { get; set; }
        public bool IsWishlisted { get; set; } = false;
        public List<string> ProductImages { get; set; } = new List<string>();
        public List<LocalizedColorDto> Fabrics { get; set; } = new List<LocalizedColorDto>();
        public List<LocalizedColorDto> Woods { get; set; } = new List<LocalizedColorDto>(); 
        public List<LocalizedProductAttributeValueResponseDto> ProductSpecification { get; set; } = new List<LocalizedProductAttributeValueResponseDto>();

    }
}
