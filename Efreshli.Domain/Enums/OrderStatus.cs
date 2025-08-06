using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Enums
{
    public enum OrderStatus
    {
        Pending=0,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
