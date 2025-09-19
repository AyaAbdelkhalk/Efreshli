using Efreshli.Application.DTOs.OrderDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get checkout preview with calculated totals
        /// </summary>
        /// <param name="couponId">Optional coupon ID to apply discount</param>
        /// <returns>Checkout preview with subtotal, shipping, discounts, and total</returns>
        [HttpGet("checkout-preview")]
        public async Task<IActionResult> GetCheckoutPreview([FromQuery] string? couponCode = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.GetCheckoutPreviewAsync(userId, couponCode);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Create a new order from user's cart
        /// </summary>
        /// <param name="createOrderDto">Order creation details</param>
        /// <returns>Created order details</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.CreateOrderAsync(userId, createOrderDto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all orders for the current user
        /// </summary>
        /// <param name="status">Optional filter by order status (0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled)</param>
        /// <returns>List of user's orders summary</returns>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int? status = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.GetUserOrdersAsync(userId, status);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get specific order details by ID
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Detailed order information</returns>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.GetOrderByIdAsync(userId, orderId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cancel a pending order
        /// </summary>
        /// <param name="orderId">Order ID to cancel</param>
        /// <returns>Success status</returns>
        [HttpPatch("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.CancelOrderAsync(userId, orderId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all orders for the current user
        /// </summary>
        /// <returns>List of all user's orders</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _orderService.GetAllOrdersAsync(userId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}