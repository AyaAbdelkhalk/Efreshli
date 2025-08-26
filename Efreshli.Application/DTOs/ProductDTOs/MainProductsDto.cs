using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs
{
    public class MainProductsDto
    {
        public int ProductId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string? DimensionsOrSize { get; set; }
        public decimal Price { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? ProductItemColorsUrls { get; set; } = new List<string>();
        public int Discount { get; set; } 
        public bool IsWishlisted { get; set; } = false;



    }
}
