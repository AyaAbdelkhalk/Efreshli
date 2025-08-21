//using Efreshli.Domain.Enums;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Efreshli.Infrastructure.Data.Seeders
//{
//    public static class RoleSeeder
//    {
//        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
//        {
//            using var scope = serviceProvider.CreateScope();
//            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//            var logger = scope.ServiceProvider.GetRequiredService<ILogger<RoleSeeder>>();

//            var roles = new[] { UserRoles.Admin, UserRoles.User, UserRoles.Vendor };

//            foreach (var role in roles)
//            {
//                if (!await roleManager.RoleExistsAsync(role))
//                {
//                    var result = await roleManager.CreateAsync(new IdentityRole(role));
//                    if (result.Succeeded)
//                    {
//                        logger.LogInformation("Created role: {Role}", role);
//                    }
//                    else
//                    {
//                        logger.LogError("Failed to create role {Role}: {Errors}",
//                            role, string.Join(", ", result.Errors.Select(e => e.Description)));
//                    }
//                }
//            }
//        }
//    }
//}
