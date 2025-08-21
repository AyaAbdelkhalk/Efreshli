using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductAttributeDTOs
{
    public class CreateProductAttributeDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CategoryId { get; set; }
    }
}
