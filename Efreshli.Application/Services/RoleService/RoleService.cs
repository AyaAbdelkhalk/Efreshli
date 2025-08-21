using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<IdentityRole> roleManager, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task EnsureRolesExistAsync()
        {
            try
            {
                await CreateRoleIfNotExistsAsync(UserRoles.Admin);
                await CreateRoleIfNotExistsAsync(UserRoles.Customer);
                //await CreateRoleIfNotExistsAsync(UserRoles.Vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure roles exist");
                throw;
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task CreateRoleIfNotExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                }

                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }
}