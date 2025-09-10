using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
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
        public string? DimensionsOrSize { get; set; }
        public string? SKU { get; set; }
        public string? Category { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
        public string? Model_3D { get; set; }
        public List<ProductAttributeValueResponseDto> ProductSpecificatoion { get; set; } = new List<ProductAttributeValueResponseDto>();
        public List<ProductItemDetailsDto> ProductItems { get; set; } = new List<ProductItemDetailsDto>();
    }
}
