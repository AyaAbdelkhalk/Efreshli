using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs
{
    public class CreateProductColorDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public IFormFile? ColorImg { get; set; }
    }
}
