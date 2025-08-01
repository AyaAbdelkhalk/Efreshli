using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Address : Auditable
    {
        [Key]
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string FullAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public bool IsDefault { get; set; }
    }
}
