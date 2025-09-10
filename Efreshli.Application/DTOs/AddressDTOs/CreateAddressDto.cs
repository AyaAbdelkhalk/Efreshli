using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.AddressDTOs
{
    public class CreateAddressDto
    {
        public string Location { get; set; }
        public string Area { get; set; }
        public string FullAddress { get; set; }
        public int FloorNumber { get; set; }
        public string PhoneNumber { get; set; }
    }
}
