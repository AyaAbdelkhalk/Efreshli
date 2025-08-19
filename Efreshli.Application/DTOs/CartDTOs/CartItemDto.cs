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
    }
}
