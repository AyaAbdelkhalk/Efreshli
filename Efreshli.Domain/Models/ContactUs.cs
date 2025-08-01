using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
