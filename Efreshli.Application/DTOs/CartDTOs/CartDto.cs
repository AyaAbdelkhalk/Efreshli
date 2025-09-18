using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CartDTOs
{
    public class CartDto
    {
        public int CartId { get; set; }
        public string ApplicationUserId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal GrandTotal { get; set; } // مجموع أسعار كل المنتجات في السلة
        public decimal Total { get; set; }

    }
}
