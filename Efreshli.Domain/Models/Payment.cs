using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Payment : Auditable
    {
        [Key]
        public int PaymentId { get; set; }
        public Order? Order { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } 
        public PaymentStatus PaymentStatus { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? TransactionId { get; set; }
    }
}
