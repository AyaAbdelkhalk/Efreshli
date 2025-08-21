using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CouponDTOs
{
    public class CouponDTO
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal? MinOrderAmount { get; set; }
    }
}
