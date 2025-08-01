using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; }

        public int Qty { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
    }
}
