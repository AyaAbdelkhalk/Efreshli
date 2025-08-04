using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class ProductAttribute : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CategoryId { get; set; }
        public virtual Brands? Category { get; set; }

        public virtual ICollection<ProductAttributeValue>? AttributeValues { get; set; }
    }
}
