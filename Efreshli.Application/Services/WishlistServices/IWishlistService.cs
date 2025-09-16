using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.WishlistServices
{ 
    public interface IWishlistService
    {
        //WishList
        Task<Response<GetWishlistDto>> CreateWishlistAsync(CreateWishlistDto createWishlistDto);
        Task<Response<List<GetWishlistDto>>> GetAllWishlistsAsync();
        Task<Response<List<GetWishlistDropDownDto>>> GetWishlistsDropDownAsync(int productId);

        Task<Response<GetWishlistDetailsDto>> GetWishlistByIdAsync(int wishlistId);
        //Task<Response<ShareWishlistDto>> ShareWishlistAsync(int wishlistId);
        Task<Response<UpdateWishlistDto>> UpdateWishlistAsync(int wishlistId, UpdateWishlistDto updateWishlistDto);
        Task<Response<GetWishlistDto>> DeleteWishlistAsync(int wishlistId);
        Task<Response<List<string>>> GetMainImagesUrlsAsync(int wishlistId);

        //WishList Items
        Task<Response<List<LocalizedGetWishlistItemDto>>> GetWishlistItemsByWishListIdAsync(int wishlistId);
        
        Task<Response<LocalizedGetWishlistItemDto>> AddItemToWishlistAsync(int wishlistId, int itemId);
        Task<Response<LocalizedGetWishlistItemDto>> RemoveItemFromWishlistAsync(int wishlistId, int itemId);
        Task<Response<bool>> IsItemWishlisted(int itemId);
        Task<Response<bool>> IsProductInWishlistAsync(int productId, int wishlistId);
    }
}
