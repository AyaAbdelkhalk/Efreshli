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
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string? BrandEn { get; set; }
        public string? BrandAr { get; set; }
        public string? DimensionsOrSize { get; set; }
        public string? SKU { get; set; }
        public string? CategoryEn { get; set; }
        public string? CategoryAr { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
        public List<ProductAttributeValueResponseDto> ProductSpecificatoion { get; set; } = new List<ProductAttributeValueResponseDto>();
        public List<ProductItemDetailsDto> ProductItems { get; set; } = new List<ProductItemDetailsDto>();
        //public List<ProductDetailsColorDto> ProductColors { get; set; } = new List<ProductDetailsColorDto>();
        //public List<ProductDetailsColorDto> ProductFabrics { get; set; } = new List<ProductDetailsColorDto>();
        //public List<ProductDetailsColorDto> ProductWoods { get; set; } = new List<ProductDetailsColorDto>();



    }
}
