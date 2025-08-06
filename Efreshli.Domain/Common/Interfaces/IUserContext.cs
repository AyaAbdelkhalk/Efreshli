using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Efreshli.Domain.Models;

namespace Efreshli.Domain.Common.Interfaces
{
    public interface IUserContext
    {
        public string? GetCurrentUserId();
        public Task<ApplicationUser?> GetCurrentUserDetailsAsync();
    }
}
