using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys;

namespace Efreshli.Application.Services.WishlistServices
{
    public class WishlistService : IWishlistService
    {
        #region Ctor
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        } 
        #endregion

        #region WishList
        public async Task<Response<GetWishlistDto>> CreateWishlistAsync(CreateWishlistDto createWishlistDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<GetWishlistDto>("User Not Found");
            }

            var wishlist = new Domain.Models.Wishlist
            {
                ApplicationUserId = userId, //////////create for current user
                Name = createWishlistDto.Name

            };
            await _unitOfWork.WishlistRepository.AddAsync(wishlist);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(new GetWishlistDto
            {
                WishlistId = wishlist.WishlistId,
                WishlistName = wishlist.Name,
                ItemsCount = 0
            });
        }

        public async Task<Response<List<GetWishlistDto>>> GetAllWishlistsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);///////////current user
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<List<GetWishlistDto>>("User Not Found");
            }
            else
            {
                var wishlists = await _unitOfWork.WishlistRepository.GetAllWithIncludeAsync(
                    w => w.ApplicationUserId == userId,  ///////////////get wishlist of current user
                    includes: new Expression<Func<Wishlist, object>>[]
                    {
                        c=>c.WishlistItems
                    }
                );
                //if 0  must craete first by default
                if (wishlists.Count()<1)
                {
                    var wishlist = new Domain.Models.Wishlist
                    {
                        ApplicationUserId = userId, //////////create first for current user
                        Name = "My Favourite"

                    };
                    await _unitOfWork.WishlistRepository.AddAsync(wishlist);
                    await _unitOfWork.SaveChangesAsync();
                    wishlists = await _unitOfWork.WishlistRepository.GetAllWithIncludeAsync(
                   w => w.ApplicationUserId == userId,
                   includes: new Expression<Func<Wishlist, object>>[]
                   {
                        c=>c.WishlistItems
                   });
                }
               
                
                var wishlistDtos = wishlists.Select(w => new GetWishlistDto
                {
                    WishlistId = w.WishlistId,
                    WishlistName = w.Name,
                    ItemsCount = w.WishlistItems.Count(),
                    wishlistItemDto = null

                }).ToList();
                return ResponseHandler.Success(wishlistDtos);
            }
        }
        public async Task<Response<GetWishlistDto>> DeleteWishlistAsync(int wishlistId)
        {
            var wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(wishlistId);
            if (wishlist == null)
            {
                return ResponseHandler.NotFound<GetWishlistDto>();
            }

            await _unitOfWork.WishlistRepository.RemoveAsync(wishlist.WishlistId);
            await _unitOfWork.SaveChangesAsync();
            var dto = new GetWishlistDto
            {
                WishlistId = wishlistId,
                WishlistName = wishlist.Name,
                ItemsCount = 0,
                wishlistItemDto = new GetWishlistItemDto()
            };
            return ResponseHandler.Success<GetWishlistDto>(dto, "Wishlist deleted successfully.");

        }

        public Task<Response<GetWishlistDto>> GetWishlistByIdAsync(int wishlistId)
        {
            throw new NotImplementedException();
        }

        public Task<Response<UpdateWishlistDto>> UpdateWishlistAsync(int wishlistId, UpdateWishlistDto updateWishlistDto)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region WishList Item

        public async Task<Response<List<GetWishlistItemDto>>> GetWishlistItemByWishListIdAsync(int wishlistId)
        {
            var items = await _unitOfWork.WishlistItemRepository.GetAllWithIncludeAsync(
                    c=>c.WishlistId == wishlistId,
                    includes: new Expression<Func<WishlistItem, object>>[]
                    {x=>x.ProductItem }
                );
            var resdto = new List<GetWishlistItemDto>();
            foreach (var item in resdto)
            {
                var wishlistItemDto = new GetWishlistItemDto
                {

                };
                resdto.Add(wishlistItemDto);
            }
            return ResponseHandler.Success(resdto);
        }
        public async Task<Response<GetWishlistItemDto>> AddItemToWishlistAsync(int wishlistId, int itemId)
        {
            var wishlistItem = new Domain.Models.WishlistItem
            {
                WishlistId = wishlistId,
                ProductItemId = itemId
            };
            await _unitOfWork.WishlistItemRepository.AddAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(new GetWishlistItemDto
            {
               
            });

        }

        public async Task<Response<bool>> IsItemWishlisted(string userId, int itemId)
        {
            var wishlist = await _unitOfWork.WishlistItemRepository.GetAllWithIncludeAsync(
                predicate: w => w.ProductItemId == itemId,
                includes: w => w.Wishlist
            );
            wishlist = wishlist.Where(w => w.Wishlist != null && w.Wishlist.ApplicationUserId == userId).ToList();
            return ResponseHandler.Success(wishlist.Any());
        }
        public Task<Response<GetWishlistItemDto>> RemoveItemFromWishlistAsync(int wishlistId, int itemId)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
