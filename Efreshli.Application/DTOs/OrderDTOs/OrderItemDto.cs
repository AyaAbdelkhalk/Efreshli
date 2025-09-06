namespace Efreshli.Application.DTOs.OrderDTOs
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
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
        
        // Additional properties for detailed view
        public string PriceDisplay => $"{Price:F2} Egp";
        public string TotalPriceDisplay => $"{TotalPrice:F2} Egp";
        public string QuantityDisplay => $"Quantity: {Quantity}";
        public string ProductDetails => $"{ProductName} - {Brand} - {Category}";
    }
}