using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Order : Auditable
    {
        [Key]
        public int OrderId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalPrice { get; set; }

        public int? CouponId { get; set; }

        public OrderStatus Status { get; set; }

        public int AddressId { get; set; }
        public Address DeliveryAddress { get; set; }
    }
}
