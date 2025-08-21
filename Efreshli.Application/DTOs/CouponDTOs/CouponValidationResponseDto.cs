using Efreshli.Application.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CouponDTOs
{
    public class CouponValidationResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public OrderCheckOutPreviewDto OrderPreview { get; set; }
    }
}
