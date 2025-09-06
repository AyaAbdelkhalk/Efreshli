using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Address : Auditable
    {
        [Key]
        public int AddressId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; } 
       
        public string Location { get; set; }
        public string Area { get; set; }
        public string FullAddress { get; set; }
        public int FloorNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; } 
    }
}
