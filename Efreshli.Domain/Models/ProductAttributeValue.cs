using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class ProductAttributeValue : Auditable
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ProductAttributeId { get; set; }
        public ProductAttribute ProductAttribute { get; set; }

        public string Value { get; set; }
    }
}
