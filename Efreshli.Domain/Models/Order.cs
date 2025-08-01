using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Order : Auditable
    {
        [Key]
        public int OrderId { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public int? CouponId { get; set; }
        public virtual Coupon? Coupon { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int? AddressId { get; set; }
        public virtual Address? DeliveryAddress { get; set; }
    }
}
