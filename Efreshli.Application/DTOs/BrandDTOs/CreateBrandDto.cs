using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.BrandDTOs
{
    public class CreateBrandDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public IFormFile? BrandImage { get; set; }
    }
}
