using Efreshli.Application.DTOs.WishlistDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.WishlistServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
