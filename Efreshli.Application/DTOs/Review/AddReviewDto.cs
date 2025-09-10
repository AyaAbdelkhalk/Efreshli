using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.Review
{
    public class AddReviewDto
    {
        public int Rate { get; set; }
        public string ReviewText { get; set; }
        public int ProductId { get; set; }
        public virtual ICollection<IFormFile>? Images { get; set; }= new List<IFormFile>();
    }
}
