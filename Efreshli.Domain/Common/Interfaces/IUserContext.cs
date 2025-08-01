using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Common.Interfaces
{
    public interface IUserContext
    {
        string? CurrentUserId { get; }
        string CurrentUserName { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
