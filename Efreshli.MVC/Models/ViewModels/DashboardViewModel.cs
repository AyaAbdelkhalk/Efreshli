using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Efreshli.MVC.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Existing properties
        public Dictionary<string, int> OrderStatusData { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PaymentMethodData { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CategoryProductCount { get; set; } = new Dictionary<string, int>();
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new List<RecentOrderViewModel>();
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        
        // New analytics properties
        // Sales data (daily/weekly/monthly)
        public Dictionary<DateTime, decimal> DailySales { get; set; } = new Dictionary<DateTime, decimal>();
        public Dictionary<DateTime, decimal> WeeklySales { get; set; } = new Dictionary<DateTime, decimal>();
        public Dictionary<DateTime, decimal> MonthlySales { get; set; } = new Dictionary<DateTime, decimal>();
        
        // Top-selling products
        public List<TopProductViewModel> TopSellingProducts { get; set; } = new List<TopProductViewModel>();
        
        // Sales by category
        public Dictionary<string, decimal> CategorySales { get; set; } = new Dictionary<string, decimal>();
        
        // Sales growth compared to last period
        public SalesGrowthViewModel SalesGrowth { get; set; } = new SalesGrowthViewModel();
        
        // Best-selling vs least-selling products
        public List<TopProductViewModel> BestSellingProducts { get; set; } = new List<TopProductViewModel>();
        public List<TopProductViewModel> LeastSellingProducts { get; set; } = new List<TopProductViewModel>();
        
        // Frequently bought together products
      //  public List<FrequentlyBoughtTogetherViewModel> FrequentlyBoughtTogether { get; set; } = new List<FrequentlyBoughtTogetherViewModel>();
        
        // Out-of-stock alerts
        public List<OutOfStockProductViewModel> OutOfStockProducts { get; set; } = new List<OutOfStockProductViewModel>();
        
        // Weekly revenue growth trend
      //  public List<RevenueTrendViewModel> WeeklyRevenueTrend { get; set; } = new List<RevenueTrendViewModel>();
        
        // Highest revenue day
        public DateTime HighestRevenueDay { get; set; }
        public decimal HighestRevenueAmount { get; set; }
    }

    public class RecentOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
    
    // New view models for analytics
    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public string Category { get; set; }
    }
    
    public class SalesGrowthViewModel
    {
        public decimal WeeklyGrowthPercentage { get; set; }
        public decimal WeeklyGrowthValue { get; set; }
        public decimal MonthlyGrowthPercentage { get; set; }
        public decimal MonthlyGrowthValue { get; set; }
    }
    
    //public class FrequentlyBoughtTogetherViewModel
    //{
    //    public string Product1Name { get; set; }
    //    public string Product2Name { get; set; }
    //    public int Frequency { get; set; }
    //}
    
    public class OutOfStockProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int LowStockThreshold { get; set; } = 5; // Default threshold
    }
    
    //public class RevenueTrendViewModel
    //{
    //    public DateTime WeekStartDate { get; set; }
    //    public decimal Revenue { get; set; }
    //    public decimal GrowthPercentage { get; set; }
    //}
}