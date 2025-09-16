using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.WishlistServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Efreshli.Application.Resources.SharedResourcesKeys;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewWishlist(CreateWishlistDto dto)
        {
            var res =await _wishlistService.CreateWishlistAsync(dto);
            return this.CreateResponse(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWishlists()
        {
            var res = await _wishlistService.GetAllWishlistsAsync();
            return this.CreateResponse(res);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateWishlist(int id,UpdateWishlistDto dto)
        {
            var res = await _wishlistService.UpdateWishlistAsync(id,dto);
            return this.CreateResponse(res);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteWishlist(int wishlistId)
        {
            var res = await _wishlistService.DeleteWishlistAsync(wishlistId);
            return this.CreateResponse(res);
        }
        [HttpGet("{wishlistId}")]
        public async Task<IActionResult> GetWishlistItemByWishListId(int wishlistId)
        {
            var res = await _wishlistService.GetWishlistByIdAsync(wishlistId);
            return this.CreateResponse(res);
        }
        [HttpPost("AddItemToWishlist")]
        public async Task<IActionResult> AddItemToWishlist(int wishlistId, int itemId)
        {
            var res = await _wishlistService.AddItemToWishlistAsync(wishlistId,itemId);
            return this.CreateResponse(res);
        }
        [HttpDelete("RemoveItemFromWishlist")]
        public async Task<IActionResult> RemoveItemFromWishlist(int wishlistId, int itemId)
        {
            var res = await _wishlistService.RemoveItemFromWishlistAsync(wishlistId,itemId);
            return this.CreateResponse(res);
        }
        [HttpGet("IsItemWishlisted")]
        public async Task<IActionResult> IsItemWishlisted(int itemId)
        {
            var res = await _wishlistService.IsItemWishlisted(itemId);
            return this.CreateResponse(res);
        }

        [HttpGet("IsProductInWishlist")]
        public async Task<IActionResult> IsProductInWishlist(int productId, int wishlistId)
        {
            var res = await _wishlistService.IsProductInWishlistAsync(productId, wishlistId);
            return this.CreateResponse(res);
        }
        [HttpGet("GetWishlistsDropDown")]
        public async Task<IActionResult> GetWishlistsDropDown(int productId)
        {
            var res = await _wishlistService.GetWishlistsDropDownAsync(productId);
            return this.CreateResponse(res);
        }

    }
}
