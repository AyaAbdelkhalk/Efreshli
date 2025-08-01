using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? TransactionId { get; set; }
    }
}
