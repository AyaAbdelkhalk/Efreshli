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
        Task<Response<GetWishlistDto>> GetWishlistByIdAsync(int wishlistId);
        //Task<Response<ShareWishlistDto>> ShareWishlistAsync(int wishlistId);
        Task<Response<UpdateWishlistDto>> UpdateWishlistAsync(int wishlistId, UpdateWishlistDto updateWishlistDto);
        Task<Response<GetWishlistDto>> DeleteWishlistAsync(int wishlistId);

        //WishList Items
        Task<Response<List<GetWishlistItemDto>>> GetWishlistItemByWishListIdAsync(int wishlistId);
        Task<Response<GetWishlistItemDto>> AddItemToWishlistAsync(int wishlistId, int itemId);
        Task<Response<GetWishlistItemDto>> RemoveItemFromWishlistAsync(int wishlistId, int itemId);
        Task<Response<bool>> IsItemWishlisted(int itemId);

    }
}
