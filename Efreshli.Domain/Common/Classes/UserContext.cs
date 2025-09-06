using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Efreshli.Domain.Common.Classes
{

    public class UserContext : IUserContext
    {
        private IHttpContextAccessor _httpContextAccessor;
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? CurrentUserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null || !user.Identity.IsAuthenticated)
                    return null;
                var userId = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                return userId;
            }
        }
        public string CurrentUserName
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null || !user.Identity.IsAuthenticated)
                    return "Anonymous";
                var userName = user.Identity.Name ?? "Anonymous";
                return userName;
            }
        }
        public bool IsAuthenticated
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user != null && user.Identity.IsAuthenticated;
            }
        }

        public bool HasPassword
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null || !user.Identity.IsAuthenticated)
                    return false;

                var hasPasswordClaim = user.FindFirst("hasPassword");
                if (hasPasswordClaim == null || string.IsNullOrEmpty(hasPasswordClaim.Value))
                    return false;

                return bool.TryParse(hasPasswordClaim.Value, out var result) && result;
            }
        }
    }
}
