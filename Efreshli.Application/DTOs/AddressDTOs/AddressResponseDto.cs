using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.AddressDTOs
{
    public class AddressResponseDto
    {
        public int AddressId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Location { get; set; }
        public string Area { get; set; }
        public string FullAddress { get; set; }
        public int FloorNumber { get; set; }
        public string PhoneNumber { get; set; }
    }
}
