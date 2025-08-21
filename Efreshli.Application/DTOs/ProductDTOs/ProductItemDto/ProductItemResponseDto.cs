using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductItemDto
{
    public class ProductItemResponseDto
    {
        public int ProductItemId { get; set; }
        public decimal Price { get; set; }
        public int? FabricColorId { get; set; }
        public int? WoodColorId { get; set; }
        public decimal? Discount { get; set; }
        public bool? IsPercentage { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public ProductColorDto? WoodColorImage { get; set; }
        public ProductColorDto? FabricColorImage { get; set; }
        public List<ProductColorDto>? ProductItemColors { get; set; } = new List<ProductColorDto>();

    }
}
