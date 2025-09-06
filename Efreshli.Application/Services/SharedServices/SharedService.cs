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
        private readonly IUserContext _userContext;

        public SharedService(IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        public async Task<Response<bool>> IsItemWishlisted(int itemId)
        {
            string userId = _userContext.CurrentUserId;
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

        public async Task<List<int>> GetWishlistedProductIdsAsync(string userId)
        {
            var wishlists = await _unitOfWork.WishlistRepository.GetWhereAsync(u => u.ApplicationUserId == userId);
            if (wishlists == null)
            {
                return new List<int>();
            }

            var wishlistedProductIds = new List<int>();
            foreach (var wishlist in wishlists)
            {
                var wishlistItems = await _unitOfWork.WishlistItemRepository.GetWhereAsync(w => w.WishlistId == wishlist.WishlistId);
                if (wishlistItems != null && wishlistItems.Count() > 0)
                {
                    wishlistedProductIds.AddRange(wishlistItems.Select(wi => wi.ProductItemId));
                }
            }

            return wishlistedProductIds.Distinct().ToList();
        }

        public async Task<Response<List<int>>> GetWishlistedProductIds(List<int> productIds)
        {
            string userId = _userContext.CurrentUserId;
            if (userId == null)
            {
                return ResponseHandler.NotFound<List<int>>("User not found");
            }

            var wishlists = await _unitOfWork.WishlistRepository.GetWhereAsync(w => w.ApplicationUserId == userId);
            if (wishlists == null)
            {
                return ResponseHandler.Success(new List<int>());
            }

            var wishlistedIds = new List<int>();
            foreach (var wishlist in wishlists)
            {
                var wishlistItems = await _unitOfWork.WishlistItemRepository.GetWhereAsync(wi => wi.WishlistId == wishlist.WishlistId && productIds.Contains(wi.ProductItemId));
                if (wishlistItems != null && wishlistItems.Any())
                {
                    wishlistedIds.AddRange(wishlistItems.Select(wi => wi.ProductItemId));
                }
            }

            return ResponseHandler.Success(wishlistedIds.Distinct().ToList());
        }

    }
}
