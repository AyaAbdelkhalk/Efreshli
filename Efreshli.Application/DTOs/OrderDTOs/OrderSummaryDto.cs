using Efreshli.Domain.Enums;

namespace Efreshli.Application.DTOs.OrderDTOs
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateOnly EstimatedDeliveryDate { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ItemsCount { get; set; }
        
        // UI-specific properties for orders list view
        public List<OrderItemSummaryDto> OrderItems { get; set; } = new List<OrderItemSummaryDto>();
        public string? CouponCode { get; set; }
        public string OrderNumber => $"Order #{OrderId}"; // e.g., "Order #12349"
        public string TotalDisplayText => $"{TotalPrice:F0} Egp ({ItemsCount} Items)"; // e.g., "78,205 Egp (12 Items)"
        public string StatusText => Status.ToString();
        public List<string> ProductImages => OrderItems?.Where(item => !string.IsNullOrEmpty(item.ProductImage)).Select(item => item.ProductImage!).ToList() ?? new List<string>();
    }
    
    public class OrderItemSummaryDto
    {
        public int ProductItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
    }
}