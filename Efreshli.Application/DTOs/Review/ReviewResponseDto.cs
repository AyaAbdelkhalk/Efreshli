using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.Review
{
    public class ReviewResponseDto
    {
        [Key]
        public int Id { get; set; }
        public int Rate { get; set; }
        public string ReviewText { get; set; }
        public int ProductId { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
    }
}
