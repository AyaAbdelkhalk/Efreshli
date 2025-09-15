
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CartDTOs
{
    public class CartItemDto
    {

        public int CartItemId { get; set; }
        public int ProductItemId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string DimensionsOrSize { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public List<string> ImageUrls { get; set; }
        public int FabricColorId { get; set; }
        public string FabricColorName { get; set; }
        public string FabricColorImageUrl { get; set; }
        public int WoodColorId { get; set; }
        public string WoodColorName { get; set; }
        public string WoodColorImageUrl { get; set; }

    }
}
