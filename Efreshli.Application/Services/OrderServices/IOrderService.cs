using Efreshli.Application.DTOs.OrderDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Helper.Pagination;

namespace Efreshli.Application.Services.OrderServices
{
    public interface IOrderService
    {
        Task<Response<OrderCheckOutPreviewDto>> GetCheckoutPreviewAsync(string userId, int? couponId = null);
        Task<Response<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto);
        Task<Response<List<OrderSummaryDto>>> GetUserOrdersAsync(string userId, int? status = null);
        Task<Response<OrderDto>> GetOrderByIdAsync(string userId, int orderId);
        Task<Response<bool>> CancelOrderAsync(string userId, int orderId);
        Task<Response<List<OrderSummaryDto>>> GetAllOrdersAsync(string userId);
    }
}