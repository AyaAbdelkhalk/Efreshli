using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs
{
    
    public class LocalizedGetWishlistItemDto
    {
        public int WishlistItemId { get; set; }
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string? DimensionsOrSize { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? ProductItemColorsUrls { get; set; } = new List<string>();
        public bool IsWishlisted { get; set; } = false;

    }
}
