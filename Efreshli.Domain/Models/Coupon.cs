using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Coupon : Auditable
    {
        [Key]
        public int CouponId { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
    }
}
