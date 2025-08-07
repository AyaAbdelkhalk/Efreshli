using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.WebsiteInfoDTOs
{
    public class UpdateWebsiteInfoDto
    {
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
