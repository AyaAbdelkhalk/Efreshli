//using Efreshli.Domain.Common.Interfaces;
//using Efreshli.Domain.Models;
//using Mapster;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Efreshli.Infrastructure.Data
//{
//    internal class UserContextService : IUserContext
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public UserContextService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
//        {
//            _httpContextAccessor = httpContextAccessor;
//            _userManager = userManager;
//        }
//        public async Task<ApplicationUser?> GetCurrentUserDetailsAsync()
//        {
//            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (string.IsNullOrEmpty(userId))
//                return null;

//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//                return null;

//            return user;
//        }

//        public string? GetCurrentUserId()
//        {
//            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

//        }

//    }
//}
