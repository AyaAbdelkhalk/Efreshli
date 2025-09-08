using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.OrderDTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int AddressId { get; set; }
        
        public int? CouponId { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        public string? Note { get; set; }
        
        public string? DeliveryNotes { get; set; }
    }
}