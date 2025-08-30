using Efreshli.Application.DTOs.CartDTOs;
using Efreshli.Application.Services.CartServices;
using Efreshli.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserContext _userContext;

        public CartController(ICartService cartService, IUserContext userContext)
        {
            _cartService = cartService;
            _userContext = userContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.GetCartByUserIdAsync(userId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _cartService.AddToCartAsync(userId, request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Update quantity of specific cart item
        /// </summary>
        /// <param name="request">Cart item ID and new quantity</param>
        /// <returns>Updated cart</returns>
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _cartService.UpdateCartItemQuantityAsync(userId, request);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

     
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.RemoveCartItemAsync(userId, cartItemId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

       
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.ClearCartAsync(userId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        
        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemsCount()
        {
            var userId = GetCurrentUserId();
            var result = await _cartService.GetCartItemsCountAsync(userId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        #region Helper Methods

     
        private string? GetCurrentUserId()
        {
            var userId = _userContext.CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return userId;
        }

        #endregion
    }
}