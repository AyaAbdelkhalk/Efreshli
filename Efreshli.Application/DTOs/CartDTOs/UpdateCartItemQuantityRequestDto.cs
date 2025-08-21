using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CartDTOs
{
    public class UpdateCartItemQuantityRequestDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
