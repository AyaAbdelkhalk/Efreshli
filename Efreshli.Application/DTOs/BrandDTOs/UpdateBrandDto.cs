using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.BrandDTOs
{
    public class UpdateBrandDto
    {
        public int BrandId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? OldImageId { get; set; }
        public IFormFile NewImage { get; set; }
    }
}
