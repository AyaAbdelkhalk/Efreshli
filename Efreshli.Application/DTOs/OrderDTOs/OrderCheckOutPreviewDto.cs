using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.OrderDTOs
{
    public class OrderCheckOutPreviewDto
    {

        public decimal SubTotalPrice { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }

    }
}
