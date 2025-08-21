using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs
{
    public class ProductDetailsColorDto
    {
        public int ColorId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? ImageUrl { get; set; }
        public string ColorType { get; set; } 
    }
}