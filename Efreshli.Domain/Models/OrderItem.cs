using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class OrderItem : Auditable
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public int ProductItemId { get; set; }
        public virtual ProductItem? ProductItem { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
