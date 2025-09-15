using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ContactUs
{
    public class BecomeAVendorRequestDto
    {
        [Required]
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string BrandName { get; set; }
        [Required]
        public string BrandCategory { get; set; }
        [Required]
        public string DelegatedPersonPosition { get; set; }
        [Required]
        public string WebsiteLink { get; set; }
        [Required]
        public string InstagramLink { get; set; }
        public IFormFile? Portfolio { get; set; } 
        public IFormFile? CommercialRegistrationAndTaxCard { get; set; }
        public bool HasTax14 { get; set; }
        public bool HasElectronicInvoices { get; set; }
    }
}
