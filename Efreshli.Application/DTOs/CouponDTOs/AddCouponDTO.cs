using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.CouponDTOs
{

    public class AddCouponDTO
    {
        //[Required(ErrorMessage = "Coupon code is required")]
        //[Required]
        //[StringLength(20, ErrorMessage = "Coupon code cannot exceed 20 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Discount value is required")]
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; } = true;

        [Range(0, 100000, ErrorMessage = "Usage limit cannot be negative and less than 100000.")]
        public int UsageLimit { get; set; }
    }
}
