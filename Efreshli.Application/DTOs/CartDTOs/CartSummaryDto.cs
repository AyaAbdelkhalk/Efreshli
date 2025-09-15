using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.DTOs.CartDTOs
{
    public class CartSummaryDto
    {
        public List<CartSummaryItemDto> Items { get; set; } = new List<CartSummaryItemDto>();
        public decimal Subtotal { get; set; }
        public decimal VATPercentage { get; set; } = 14; // Default 14% VAT
        public decimal VATAmount { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
        public string VATText { get; set; } = "VAT (14%)";
        public string ShippingText { get; set; } = "Shipping";
        
        // خصائص للعرض المنسق
        public string FormattedSubtotal => $"{Subtotal:N0} EGP";
        public string FormattedVATAmount => $"{VATAmount:N0} EGP";
        public string FormattedShippingPrice => $"{ShippingPrice:N0} EGP";
        public string FormattedTotal => $"{Total:N0} EGP";
        public string VATDisplayText => VATAmount > 0 ? "Included in the products" : "No VAT applicable";
        public string ShippingDisplayText => ShippingPrice > 0 ? "Calculated at the next step." : "Free shipping";
    }

    public class CartSummaryItemDto
    {
        public int CartItemId { get; set; }
        public int ProductItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } // السعر للقطعة الواحدة
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } // السعر الإجمالي للكمية
        public string? ProductImage { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        
        // معلومات إضافية تظهر في الصورة
        public string? Dimensions { get; set; } // مثل "D 45 x 38 H cm"
        public string? Material { get; set; } // مثل "Fabric: boucle offwhite"
        public string? ProductStatus { get; set; } // مثل "Made to Order" أو "Ready to Ship"
        public string? Description { get; set; } // وصف إضافي مثل "Height 30 cm"
        public string? Brand { get; set; }
        public string? Category { get; set; }
        
        // خصائص للعرض
        public string FormattedUnitPrice => $"{UnitPrice:N0} EGP";
        public string FormattedTotalPrice => $"{TotalPrice:N0} EGP";
        public string QuantityText => $"Quantity: {Quantity}";
    }
}