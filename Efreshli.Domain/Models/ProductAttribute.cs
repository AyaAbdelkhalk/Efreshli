using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductAttributeValue> AttributeValues { get; set; }
    }
}
