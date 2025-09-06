using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.SharedServices
{
    public class SharedService : ISharedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SharedService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<bool>> IsItemWishlisted(int itemId)
        {
            string userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.NotFound<bool>("User not found");
            }
            var wishlists = await _unitOfWork.WishlistRepository.GetWhereAsync(u=>u.ApplicationUserId==userId);
            if (wishlists == null)
            {
                return ResponseHandler.NotFound<bool>("Wishlist not found");
            }
            foreach (var wishlist in wishlists)
            {
                var wishlistItems = await _unitOfWork.WishlistItemRepository.GetWhereAsync(w => w.WishlistId == wishlist.WishlistId && w.ProductItemId == itemId);
                if (wishlistItems != null && wishlistItems.Count() > 0)
                {
                    return ResponseHandler.Success(true);
                }
            }
            return ResponseHandler.Success(false);
        }
    }
}
