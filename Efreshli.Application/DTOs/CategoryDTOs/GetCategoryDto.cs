using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CategoryDTOs
{
    public class GetCategoryDto
    {
        public int CategoryId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? ParentId { get; set; }
        public int? ImageId { get; set; }
        public string? ImageUrl { get; set; }
        public GetCategoryDto? Parent { get; set; }
        public string? CreatedBy { get; set; }
        public int ProductCount { get; set; } 
        public List<GetCategoryDto>? Children { get; set; } = new List<GetCategoryDto>();

    }
}
