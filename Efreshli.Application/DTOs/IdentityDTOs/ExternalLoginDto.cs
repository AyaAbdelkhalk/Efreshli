using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.IdentityDTOs
{
    public class ExternalLoginDto
    {
        public string Provider { get; set; }
        public string Token { get; set; }
    }
}
