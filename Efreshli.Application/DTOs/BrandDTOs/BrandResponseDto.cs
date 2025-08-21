using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.BrandDTOs
{
    public class BrandResponseDto
    {
        public int BrandId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? ImageUrl { get; set; }
        public int? ImageId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }
}
