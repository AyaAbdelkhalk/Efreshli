using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs
{
    public class CreateProductDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string? DimensionsOrSize { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        //public List<CreateProductColorDto>? ProductColors { get; set; } = new List<CreateProductColorDto>();
        public List<CreateProductAttributeValueDto>? ProductAttributeValues { get; set; } = new List<CreateProductAttributeValueDto>();
        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
        public List<CreateProductItemDto>? ProductItems { get; set; } = new List<CreateProductItemDto>();



    }
}
