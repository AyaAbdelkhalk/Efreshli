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

        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }

        public decimal SubTotalPrice { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        public DateOnly EstimatedDeliveryDate {  get; set; }
        public int? CouponId { get; set; }
        public virtual Coupon? Coupon { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int? AddressId { get; set; }
        public virtual Address? DeliveryAddress { get; set; }
        [ForeignKey(nameof(Payment))]
        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
    }

}
