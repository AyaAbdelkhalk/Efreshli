using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs
{
    public class FilteredProductsDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? DimensionsOrSize { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }

        public int? BrandId { get; set; }

        public decimal Price { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? ProductItemColorsUrls { get; set; } = new List<string>();
        public decimal Discount { get; set; }
        public bool IsWishlisted { get; set; } = false;



    }
}
