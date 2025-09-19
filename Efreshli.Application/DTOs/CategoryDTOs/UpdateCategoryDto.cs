using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {
        public int CategoryId { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public int? ParentId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
