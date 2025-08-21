using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Product : Auditable
    {
        [Key]
        public int ProductId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string? DimensionsOrSize { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public virtual ICollection<Image>? ProductImages { get; set; }
        public virtual ICollection<ProductAttributeValue>? AttributeValues { get; set; }
    }
}
