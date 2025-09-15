using Efreshli.Application.DTOs.CartDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.CartServices
{
    public interface ICartService
    {
        Task<Response<CartDto>> GetCartByUserIdAsync(string userId);
        Task<Response<CartDto>> AddToCartAsync(string userId, AddToCartRequestDto request);
        Task<Response<CartDto>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemQuantityRequestDto request);
        Task<Response<bool>> RemoveCartItemAsync(string userId, int cartItemId);
        Task<Response<bool>> ClearCartAsync(string userId);
        Task<Response<int>> GetCartItemsCountAsync(string userId);
        Task<decimal> GetGrandTotalOfCart(string userId);

    }
}
