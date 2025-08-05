using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Efreshli.Domain.Common.Classes
{

    public static class UserContext
    {
        private static IServiceProvider? _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static string? CurrentUserId
        {
            get
            {
                using var scope = _serviceProvider?.CreateScope();
                var httpContextAccessor = scope?.ServiceProvider.GetService<IHttpContextAccessor>();
                var userManager = scope?.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                
                var user = httpContextAccessor?.HttpContext?.User;
                return user != null ? userManager?.GetUserId(user) : null;
            }
        }

        public static string? CurrentUserName
        {
            get
            {
                using var scope = _serviceProvider?.CreateScope();
                var httpContextAccessor = scope?.ServiceProvider.GetService<IHttpContextAccessor>();
                var userManager = scope?.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                
                var user = httpContextAccessor?.HttpContext?.User;
                return user != null ? userManager?.GetUserName(user) : null;
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                using var scope = _serviceProvider?.CreateScope();
                var httpContextAccessor = scope?.ServiceProvider.GetService<IHttpContextAccessor>();
                
                var user = httpContextAccessor?.HttpContext?.User;
                return user?.Identity?.IsAuthenticated ?? false;
            }
        }

        public static ApplicationUser? ApplicationUser
        {
            get
            {
                using var scope = _serviceProvider?.CreateScope();
                var httpContextAccessor = scope?.ServiceProvider.GetService<IHttpContextAccessor>();
                var userManager = scope?.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                
                var user = httpContextAccessor?.HttpContext?.User;
                return user != null ? userManager?.GetUserAsync(user).Result : null;
            }
        }

        // Alternative method for async operations
        public static async Task<ApplicationUser?> GetApplicationUserAsync()
        {
            using var scope = _serviceProvider?.CreateScope();
            var httpContextAccessor = scope?.ServiceProvider.GetService<IHttpContextAccessor>();
            var userManager = scope?.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            
            var user = httpContextAccessor?.HttpContext?.User;
            return user != null ? await userManager?.GetUserAsync(user)! : null;
        }
    }
}
