using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.RoleService
{
    public interface IRoleService
    {
        Task EnsureRolesExistAsync();
        Task<bool> RoleExistsAsync(string roleName);
        Task CreateRoleIfNotExistsAsync(string roleName);
    }
}
