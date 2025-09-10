using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class VendorRequest : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BrandName { get; set; }
        public string BrandCategory { get; set; }
        public string DelegatedPersonPosition { get; set; }
        public string WebsiteLink { get; set; }
        public string InstagramLink { get; set; }
        public string Portfolio { get; set; } // file
        public string CommercialRegistrationAndTaxCard { get; set; }
        public bool HasTax14 { get; set; }
        public bool HasElectronicInvoices { get; set; }
    }
}
