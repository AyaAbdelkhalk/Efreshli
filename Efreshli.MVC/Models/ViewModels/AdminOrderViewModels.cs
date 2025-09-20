using Efreshli.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Efreshli.MVC.Models.ViewModels
{
    public class AdminOrderListViewModel
    {
        public List<AdminOrderListItemViewModel> Orders { get; set; } = new List<AdminOrderListItemViewModel>();
        public AdminOrderFilterViewModel Filters { get; set; } = new AdminOrderFilterViewModel();
    }

    public class AdminOrderListItemViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string ShippingType { get; set; } = "Delivery"; // Default to Delivery
        public OrderStatus Status { get; set; }
    }

    public class AdminOrderFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? ShippingType { get; set; }
    }

    public class AdminOrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        // Customer Info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;

        // Products
        public List<AdminOrderProductItemViewModel> Products { get; set; } = new List<AdminOrderProductItemViewModel>();

        // Order Summary
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalTotal { get; set; }

        // Extra Info
        public PaymentMethod PaymentMethod { get; set; }
        public string CustomerNotes { get; set; } = string.Empty;
    }

    public class AdminOrderProductItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class UpdateOrderStatusViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }
    }
}