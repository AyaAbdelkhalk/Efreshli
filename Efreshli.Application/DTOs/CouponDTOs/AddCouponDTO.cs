using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.CouponDTOs
{

    public class AddCouponDTO
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(20, ErrorMessage = "Coupon code cannot exceed 20 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Discount value is required")]
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; } = true;

        [Range(0, int.MaxValue, ErrorMessage = "Usage limit cannot be negative")]
        public int UsageLimit { get; set; }
    }
}
