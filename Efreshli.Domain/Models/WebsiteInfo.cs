using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class WebsiteInfo : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string LogoURL { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Office { get; set; }
        public string Location { get; set; }
        public string FacebookLink { get; set; }
        public string InstagramLink { get; set; }
        public string YoutubeLink { get; set; }
        public string LinkedinLink { get; set; }
    }
}
