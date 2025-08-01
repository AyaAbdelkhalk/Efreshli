using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Brand: Auditable
    {
        [Key]
        public int BrandId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
    }
}
