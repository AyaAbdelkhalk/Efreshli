using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs
{
    public class DropDownDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; } = 0;
    }
    public class ColorsDropDownDto
    {
        public int ColorId { get; set; }
        public string ImageUrl { get; set; }
    }
}
