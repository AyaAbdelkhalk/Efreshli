using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EfreshliDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (context.Database.EnsureCreated())
            {
                string[] roles = { "Admin", "Customer"};
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
                if (!context.Users.Any())
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        PhoneNumber = "1234567890",
                        FirstName = "Admin",
                        LastName = "User",
                        EmailConfirmed = true,
                        CreatedBy = "System",
                        IsDeleted = false

                    };
                    await userManager.CreateAsync(adminUser, "Admin@123");
                    await userManager.AddToRoleAsync(adminUser, "Admin");   

                    var customerUser = new ApplicationUser
                    {
                        UserName = "customer",
                        Email = "customer@example.com",
                        PhoneNumber = "0987654321",
                        FirstName = "Customer",
                        LastName = "User",
                        EmailConfirmed = true,
                        CreatedBy = "System",
                        IsDeleted = false

                    };
                    await userManager.CreateAsync(customerUser, "Customer@123");
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                }
                if (context.ChangeTracker.HasChanges())
                {
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine("Database already exists, skipping seed.");
            }

        }
    }
}
