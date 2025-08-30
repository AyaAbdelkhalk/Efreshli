using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.ProductServices;
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
        private readonly IProductService _productService;


        public WishlistService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
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
                ItemsCount = 0,
                wishlistItemsDto = new List<GetWishlistItemDto>(),
                WishlistUrl = $"api/wishlist/{wishlist.WishlistId}"
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
                        Name = "My Favorites"

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
                    MainImages = GetMainImagesUrlsAsync(w.WishlistId).Result.Data,
                    wishlistItemsDto = GetWishlistItemByWishListIdAsync(w.WishlistId).Result.Data,
                    WishlistUrl = $"api/wishlist/{w.WishlistId}"

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
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<GetWishlistDto>("User Not Found");
            }
            if (wishlist.ApplicationUserId != userId)
            {
                return ResponseHandler.Unauthorized<GetWishlistDto>();
            }
            try
            {
                await _unitOfWork.WishlistRepository.RemoveAsync(wishlist.WishlistId);
                await _unitOfWork.SaveChangesAsync();
                var dto = new GetWishlistDto
                {
                    WishlistId = wishlistId,
                    WishlistName = wishlist.Name,
                    ItemsCount = 0,
                    wishlistItemsDto = new List<GetWishlistItemDto>(),
                    WishlistUrl = $"api/wishlist/{wishlist.WishlistId}"
                };
                return ResponseHandler.Success<GetWishlistDto>(dto, "Wishlist deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<GetWishlistDto>($"An error occurred while deleting the wishlist. {ex.Message}");
            }

        }

        public async Task<Response<GetWishlistDto>> GetWishlistByIdAsync(int wishlistId)
        {
            var wishlist = _unitOfWork.WishlistRepository.GetByIdWithIncludeAsync(
                wishlistId
                , includes: new Expression<Func<Wishlist, object>>[]
                {
                    c=>c.WishlistItems
                }
                );
            if (wishlist == null)
            {
                return ResponseHandler.NotFound<GetWishlistDto>("Wishlist not found");
            }
            try
            {
                var dto = new GetWishlistDto
                {
                    WishlistId = wishlist.Result.WishlistId,
                    WishlistName = wishlist.Result.Name,
                    ItemsCount = wishlist.Result.WishlistItems.Count(),
                    wishlistItemsDto = GetWishlistItemByWishListIdAsync(wishlistId).Result.Data,
                    WishlistUrl = $"api/wishlist/{wishlist.Result.WishlistId}"
                    
                };
                return ResponseHandler.Success(dto);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<GetWishlistDto>($"An error occurred while retrieving the wishlist. {ex.Message}");
            }
            

        }

        public async Task<Response<UpdateWishlistDto>> UpdateWishlistAsync(int wishlistId, UpdateWishlistDto updateWishlistDto)
        {
            var wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(wishlistId);
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<UpdateWishlistDto>("User Not Found");
            }
            if (wishlist == null)
            {
                return ResponseHandler.NotFound<UpdateWishlistDto>();
            }
            if (wishlist.ApplicationUserId != userId)
            {
                return ResponseHandler.Unauthorized<UpdateWishlistDto>();
            }
            try
            {
                wishlist.Name = updateWishlistDto.Name;
                await _unitOfWork.WishlistRepository.UpdateAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();
                return ResponseHandler.Success(new UpdateWishlistDto
                {
                    Id=wishlist.WishlistId,
                    Name = wishlist.Name
                });
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<UpdateWishlistDto>($"An error occurred while updating the wishlist. {ex.Message}");
            }


        }

        #endregion

        #region WishList Item

        public async Task<Response<List<GetWishlistItemDto>>> GetWishlistItemByWishListIdAsync(int wishlistId)
        {
            var wishlistItems = await _unitOfWork.WishlistItemRepository.GetAllWithIncludeAsync(
                w => w.WishlistId == wishlistId,
                includes: new Expression<Func<WishlistItem, object>>[]
                {
                    c=>c.ProductItem!
                }
                );
            if (wishlistItems == null || wishlistItems.Count() == 0)
            {
                return ResponseHandler.Success(new List<GetWishlistItemDto>());
            }
            var wishlistItemDtos = new List<GetWishlistItemDto>();
            foreach (var item in wishlistItems)
            {
                var productItem = await _productService.GetWishlistItemsForUserAsync(item.ProductItemId);
                if (productItem.Succeeded && productItem.Data != null)
                {
                    var dtores = await _productService.GetWishlistItemsForUserAsync(item.ProductItemId);
                    var dto = dtores.Data;
                    dto.WishlistId= wishlistId;
                    dto.WishlistItemId= item.WishlistItemId;
                    dto.ProductId = item.ProductItemId;
                    dto.IsWishlisted = IsItemWishlisted(item.ProductItemId).Result.Data;
                    wishlistItemDtos.Add(dto);

                }
            }
            return ResponseHandler.Success(wishlistItemDtos);
        }
        public async Task<Response<GetWishlistItemDto>> AddItemToWishlistAsync(int wishlistId, int itemId)
        {
            var wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(wishlistId);
            if (wishlist == null)
            {
                return ResponseHandler.NotFound<GetWishlistItemDto>("Wishlist not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<GetWishlistItemDto>("User Not Found");
            }
            if (wishlist.ApplicationUserId != userId)
            {
                return ResponseHandler.Unauthorized<GetWishlistItemDto>();
            }
            var existingItems = await _unitOfWork.WishlistItemRepository.GetWhereAsync(w => w.WishlistId == wishlistId && w.ProductItemId == itemId);
            if (existingItems != null && existingItems.Count() > 0)
            {
                return ResponseHandler.BadRequest<GetWishlistItemDto>("Already Exists In this Wishlist");
            }

            var wishlistItem = new Domain.Models.WishlistItem
            {
                WishlistId = wishlistId,
                ProductItemId = itemId
            };

            await _unitOfWork.WishlistItemRepository.AddAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(new GetWishlistItemDto
            {
                WishlistId = wishlistId,
                ProductId = itemId,
                WishlistItemId = wishlistItem.WishlistItemId

            });

        }

        public async Task<Response<bool>> IsItemWishlisted( int itemId)
        {
            string userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<bool>("User Not Found");
            }
            var wishlists = await _unitOfWork.WishlistRepository.GetWhereAsync(w => w.ApplicationUserId == userId);
            if (wishlists == null || wishlists.Count() == 0)
            {
                return ResponseHandler.Success(false);
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
        public async Task<Response<GetWishlistItemDto>> RemoveItemFromWishlistAsync(int wishlistId, int itemId)
        {
            var wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(wishlistId);
            if (wishlist == null)
            {
                return ResponseHandler.NotFound<GetWishlistItemDto>("Wishlist not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return ResponseHandler.Unauthorized<GetWishlistItemDto>("User Not Found");
            }
            if (wishlist.ApplicationUserId != userId)
            {
                return ResponseHandler.Unauthorized<GetWishlistItemDto>();
            }


            var wishlistItems = await _unitOfWork.WishlistItemRepository.GetWhereAsync(w => w.WishlistId == wishlistId && w.ProductItemId == itemId);
            if (wishlistItems == null)
            {
                return ResponseHandler.NotFound<GetWishlistItemDto>("Item not found in wishlist");
            }
            var wishlistItem = wishlistItems.FirstOrDefault();
            if (wishlistItem != null)
            {
                try
                {
                    await _unitOfWork.WishlistItemRepository.RemoveAsync(wishlistItem.WishlistItemId);
                    await _unitOfWork.SaveChangesAsync();
                    return ResponseHandler.Success(new GetWishlistItemDto
                    {
                        WishlistId = wishlistId,
                        ProductId = itemId,
                        WishlistItemId = wishlistItem.WishlistItemId
                    });
                }
                catch (Exception ex)
                {
                    return ResponseHandler.BadRequest<GetWishlistItemDto>($"An error occurred while removing the item from the wishlist. {ex.Message}");
                }

            }
            return ResponseHandler.NotFound<GetWishlistItemDto>();
        }

        public async Task<Response<List<string>>> GetMainImagesUrlsAsync(int wishlistId)
        {
            var wishlistItems = await _unitOfWork.WishlistItemRepository.GetAllWithIncludeAsync(
                w => w.WishlistId == wishlistId,
                includes: new Expression<Func<WishlistItem, object>>[]
                {
                    c=>c.ProductItem!
                }
                );
            if (wishlistItems == null || wishlistItems.Count() == 0)
            {
                return ResponseHandler.Success(new List<string>());
            }
            var imageUrls = new List<string>();
            foreach (var item in wishlistItems)
            {
                var productItem = await _productService.GetWishlistItemsForUserAsync(item.ProductItemId);
                if (productItem.Succeeded && productItem.Data != null)
                {
                    var dtores = await _productService.GetWishlistItemsForUserAsync(item.ProductItemId);
                    var dto = dtores.Data;
                    if (!string.IsNullOrEmpty(dto.ImageUrl))
                    {
                        imageUrls.Add(dto.ImageUrl);
                    }
                }
            }
            return ResponseHandler.Success(imageUrls);

        }

        #endregion



    }
}
