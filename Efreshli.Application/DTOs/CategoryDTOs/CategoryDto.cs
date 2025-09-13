using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CategoryDTOs
{
    public class LocalizedCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        //public List<LocalizedCategoryDto> SubCategories { get; set; } = new();
        public bool HasSubCategories { get; set; }
    }
}
