using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class WebsiteInfo : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string FacebookLink { get; set; }
        public string InstaLink { get; set; }
        public string YoutubeLink { get; set; }
        public string LinkedinLink { get; set; }
    }
}
