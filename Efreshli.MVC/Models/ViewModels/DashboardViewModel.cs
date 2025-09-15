using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Efreshli.MVC.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Order Status Chart Data
        public Dictionary<string, int> OrderStatusData { get; set; } = new Dictionary<string, int>();
        
        // Payment Method Chart Data
        public Dictionary<string, int> PaymentMethodData { get; set; } = new Dictionary<string, int>();
        
        // Category Distribution Chart Data
        public Dictionary<string, int> CategoryProductCount { get; set; } = new Dictionary<string, int>();
        
        // Recent Orders Data
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new List<RecentOrderViewModel>();
        
        // Summary Statistics
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RecentOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}