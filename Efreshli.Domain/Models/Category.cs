using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Brands : Auditable
    {
        [Key]
        public int CategoryId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Brands? Parent { get; set; }

        public int? ImageId { get; set; }
        public virtual Image? Image { get; set; }
    }
}
