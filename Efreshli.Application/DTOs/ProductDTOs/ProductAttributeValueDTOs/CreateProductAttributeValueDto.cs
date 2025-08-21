using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs
{
    public class CreateProductAttributeValueDto
    {
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public string Value { get; set; }
    }
}
