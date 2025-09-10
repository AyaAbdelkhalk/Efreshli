using Efreshli.Domain.Enums;

namespace Efreshli.Application.DTOs.OrderDTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal SubTotalPrice { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        public DateOnly EstimatedDeliveryDate { get; set; }
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
        public OrderStatus Status { get; set; }
        public int? AddressId { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // UI-specific properties for order details view
        public string OrderNumber => $"Order #{OrderId}"; // e.g., "Order #12349"
        public string OrderIdDisplay => $"#{OrderId}"; // e.g., "#1"
        public string OrderDateDisplay => CreatedDate.ToString("yyyy-MM-dd"); // e.g., "2024-09-26"
        public string StatusText => Status.ToString();
        public string PaymentMethodText => PaymentMethod.ToString().Replace("CashOnDelivery", "Cash On Delivery");
        public string PaymentStatusText => PaymentStatus.ToString();
        public int NumberOfProducts => OrderItems?.Sum(item => item.Quantity) ?? 0;
        public string TotalAmountDisplay => $"{TotalPrice:F2} Egp";
        public bool CanBeCancelled => Status == OrderStatus.Pending;
        
        // Product customization details
        public List<ProductCustomizationDto> ProductCustomizations => OrderItems?.Select(item => new ProductCustomizationDto
        {
            ProductName = item.ProductName,
            Fabric = item.Color, // Using Color as Fabric since we store fabric color
            Color = item.Color,
            Brand = item.Brand,
            Category = item.Category,
            Quantity = item.Quantity,
            Price = item.Price,
            ProductImage = item.ProductImage
        }).ToList() ?? new List<ProductCustomizationDto>();
    }
    
    public class ProductCustomizationDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string? Fabric { get; set; }
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductImage { get; set; }
    }
}