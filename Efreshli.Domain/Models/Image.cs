using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Models
{
    public class Image : Auditable
    {
        [Key]
        public int Id { get; set; }
        public string URL { get; set; }
        public string? PublicId { get; set; }
        public ImageReferenceType? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
    }


 
}
