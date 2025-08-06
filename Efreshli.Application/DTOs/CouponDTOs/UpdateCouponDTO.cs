namespace Efreshli.Application.DTOs.CouponDTOs
{
    public class UpdateCouponDTO
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
    }
}
