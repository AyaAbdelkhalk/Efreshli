using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Color : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public int? ImageId { get; set; }
        public Image? Image { get; set; }

        public ColorType ColorType { get; set; }
    }
}
