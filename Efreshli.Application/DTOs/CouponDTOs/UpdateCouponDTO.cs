using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.CouponDTOs
{
    public class UpdateCouponDTO
    {
        public int CouponId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]

        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int UsageLimit { get; set; }
    }
}
