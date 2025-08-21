using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductItemDto
{
    public class CreateProductItemDto
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        public bool? IsPercentage { get; set; } = false;
        public CreateProductColorDto? WoodColorImage { get; set; }
        public CreateProductColorDto? FabricColorImage { get; set; }
        public List<CreateProductColorDto>? ProductItemColors { get; set; } = new List<CreateProductColorDto>();
    }
}
