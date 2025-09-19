using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Efreshli.MVC.Models;
using Efreshli.MVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace Efreshli.MVC.Controllers
{
    [Authorize(UserRoles.Admin)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await GetDashboardDataAsync();
            ViewData["Title"] = "Dashboard";
            return View(dashboardData);
        }
        
        private async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var model = new DashboardViewModel();
            
            // Get all necessary data
            var orders = (await _unitOfWork.OrderRepository.GetAllAsync()).ToList();
            var orderItems = orders.SelectMany(o => o.OrderItems ?? new List<OrderItem>()).ToList();
            var products = (await _unitOfWork.ProductRepository.GetAllAsync()).ToList();
            var productItems = products.SelectMany(p => p.ProductItems ?? new List<ProductItem>()).ToList();
            var categories = (await _unitOfWork.CategoryRepository.GetAllAsync()).ToList();
            var users = (await _unitOfWork.UserRepository.GetAllAsync()).ToList();
            var payments = (await _unitOfWork.PaymentRepository.GetAllAsync()).ToList();
            
            // Existing data
            model.OrderStatusData = orders
                .GroupBy(o => o.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            model.PaymentMethodData = payments
                .GroupBy(p => p.PaymentMethod.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            

            model.CategoryProductCount = categories
                .DistinctBy(c => CultureInfo.CurrentUICulture.Name == "ar-EG" ? c.NameAr : c.NameEn)
                .ToDictionary(
                    c => CultureInfo.CurrentUICulture.Name == "ar-EG" ? c.NameAr : c.NameEn,
                    c => products.Count(p => p.CategoryId == c.CategoryId)
                );

            // Get recent orders
            model.RecentOrders = orders
                .OrderByDescending(o => o.CreatedDate)
                .Take(5)
                .Select(o => new RecentOrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.ApplicationUser?.FullName ?? "Unknown",
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    OrderDate = o.CreatedDate
                })
                .ToList();
            
            model.TotalProducts = products.Count();
            model.TotalOrders = orders.Count();
            model.TotalUsers = users.Count();
            model.TotalRevenue = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalPrice);
            
            // New analytics data
            // Sales data (daily/weekly/monthly)
            model.DailySales = GetDailySales(orders);
            model.WeeklySales = GetWeeklySales(orders);
            model.MonthlySales = GetMonthlySales(orders);
            
            // Top-selling products
            model.TopSellingProducts = GetTopSellingProducts(orderItems, products);
            
            // Sales by category
            model.CategorySales = GetCategorySales(orderItems, products, categories);
            
            // Sales growth compared to last period
            model.SalesGrowth = GetSalesGrowth(orders);
            
            // Best-selling vs least-selling products
            var allProductsSales = GetAllProductsSales(orderItems, products);
            model.BestSellingProducts = allProductsSales.OrderByDescending(p => p.Revenue).Take(5).ToList();
            model.LeastSellingProducts = allProductsSales.OrderBy(p => p.Revenue).Take(5).ToList();
            
            // Frequently bought together products
            model.FrequentlyBoughtTogether = GetFrequentlyBoughtTogether(orders);
            
            // Out-of-stock alerts
            model.OutOfStockProducts = GetOutOfStockProducts(productItems, products);
            
            // Order status breakdown
            model.OrderStatusBreakdown = orders
                .GroupBy(o => o.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Weekly revenue growth trend
            model.WeeklyRevenueTrend = GetWeeklyRevenueTrend(orders);
            
            // Highest revenue day
            var dailyRevenue = orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .GroupBy(o => o.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalPrice) })
                .OrderByDescending(x => x.Revenue)
                .FirstOrDefault();
                
            if (dailyRevenue != null)
            {
                model.HighestRevenueDay = dailyRevenue.Date;
                model.HighestRevenueAmount = dailyRevenue.Revenue;
            }
            
            return model;
        }
        
        private DateTime GetWeekStartDate(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            return date.AddDays(-dayOfWeek).Date;
        }
        
        private List<TopProductViewModel> GetTopSellingProducts(List<OrderItem> orderItems, List<Efreshli.Domain.Models.Product> products)
        {
            var productSales = orderItems
                .GroupBy(oi => oi.ProductItem?.Product?.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .Where(x => x.ProductId.HasValue)
                .OrderByDescending(x => x.Revenue)
                .Take(10);
            
            var result = new List<TopProductViewModel>();
            foreach (var sale in productSales)
            {
                var product = products.FirstOrDefault(p => p.ProductId == sale.ProductId);
                if (product != null)
                {
                    result.Add(new TopProductViewModel
                    {
                        ProductId = product.ProductId,
                        ProductName = CultureInfo.CurrentUICulture.Name == "ar-EG" ? product.NameAr : product.NameEn,
                        UnitsSold = sale.UnitsSold,
                        Revenue = sale.Revenue,
                        Category = product.Category != null ? 
                            (CultureInfo.CurrentUICulture.Name == "ar-EG" ? product.Category.NameAr : product.Category.NameEn) : "N/A"
                    });
                }
            }
            
            return result;
        }
        
        private Dictionary<string, decimal> GetCategorySales(List<OrderItem> orderItems, List<Efreshli.Domain.Models.Product> products, List<Category> categories)
        {
            var categorySales = orderItems
                .Where(oi => oi.ProductItem?.Product != null)
                .GroupBy(oi => oi.ProductItem.Product.CategoryId)
                .Select(g => new {
                    CategoryId = g.Key,
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .ToDictionary(x => x.CategoryId, x => x.Revenue);
            
            var result = new Dictionary<string, decimal>();
            foreach (var category in categories)
            {
                var categoryName = CultureInfo.CurrentUICulture.Name == "ar-EG" ? category.NameAr : category.NameEn;
                result[categoryName] = categorySales.ContainsKey(category.CategoryId) ? categorySales[category.CategoryId] : 0;
            }
            
            return result;
        }
        
        private List<TopProductViewModel> GetAllProductsSales(List<OrderItem> orderItems, List<Efreshli.Domain.Models.Product> products)
        {
            var productSales = orderItems
                .Where(oi => oi.ProductItem?.Product != null)
                .GroupBy(oi => oi.ProductItem.Product.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .ToDictionary(x => x.ProductId, x => new { x.UnitsSold, x.Revenue });
            
            var result = new List<TopProductViewModel>();
            foreach (var product in products)
            {
                var salesData = productSales.ContainsKey(product.ProductId) ? productSales[product.ProductId] : new { UnitsSold = 0, Revenue = (decimal)0 };
                result.Add(new TopProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = CultureInfo.CurrentUICulture.Name == "ar-EG" ? product.NameAr : product.NameEn,
                    UnitsSold = salesData.UnitsSold,
                    Revenue = salesData.Revenue,
                    Category = product.Category != null ? 
                        (CultureInfo.CurrentUICulture.Name == "ar-EG" ? product.Category.NameAr : product.Category.NameEn) : "N/A"
                });
            }
            
            return result;
        }
        
        private List<FrequentlyBoughtTogetherViewModel> GetFrequentlyBoughtTogether(List<Efreshli.Domain.Models.Order> orders)
        {
            var result = new List<FrequentlyBoughtTogetherViewModel>();
            
            // Group orders by OrderId to get items in each order
            var ordersWithItems = orders
                .Where(o => o.OrderItems != null && o.OrderItems.Count > 1)
                .Select(o => new {
                    OrderId = o.OrderId,
                    ProductIds = o.OrderItems.Select(oi => oi.ProductItem?.Product?.ProductId).Where(id => id.HasValue).Select(id => id.Value).ToList()
                })
                .ToList();
            
            // Count pairs of products bought together
            var productPairs = new Dictionary<(int, int), int>();
            
            foreach (var order in ordersWithItems)
            {
                // Get all unique pairs of products in this order
                for (int i = 0; i < order.ProductIds.Count; i++)
                {
                    for (int j = i + 1; j < order.ProductIds.Count; j++)
                    {
                        var productId1 = order.ProductIds[i];
                        var productId2 = order.ProductIds[j];
                        
                        // Create a consistent pair (smaller id first)
                        var pair = productId1 < productId2 ? (productId1, productId2) : (productId2, productId1);
                        
                        if (productPairs.ContainsKey(pair))
                        {
                            productPairs[pair]++;
                        }
                        else
                        {
                            productPairs[pair] = 1;
                        }
                    }
                }
            }
            
            // Get top 10 frequently bought together pairs
            var topPairs = productPairs
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .ToList();
            
            // Convert to view models
            var allProducts = orders
                .SelectMany(o => o.OrderItems ?? new List<OrderItem>())
                .Where(oi => oi.ProductItem?.Product != null)
                .Select(oi => oi.ProductItem.Product)
                .GroupBy(p => p.ProductId)
                .Select(g => g.First())
                .ToDictionary(p => p.ProductId, p => p);
            
            foreach (var pair in topPairs)
            {
                if (allProducts.ContainsKey(pair.Key.Item1) && allProducts.ContainsKey(pair.Key.Item2))
                {
                    var product1 = allProducts[pair.Key.Item1];
                    var product2 = allProducts[pair.Key.Item2];
                    
                    result.Add(new FrequentlyBoughtTogetherViewModel
                    {
                        Product1Name = CultureInfo.CurrentUICulture.Name == "ar-EG" ? product1.NameAr : product1.NameEn,
                        Product2Name = CultureInfo.CurrentUICulture.Name == "ar-EG" ? product2.NameAr : product2.NameEn,
                        Frequency = pair.Value
                    });
                }
            }
            
            return result;
        }
        
        private List<OutOfStockProductViewModel> GetOutOfStockProducts(List<ProductItem> productItems, List<Efreshli.Domain.Models.Product> products)
        {
            var lowStockThreshold = 5; // Default threshold
            
            // Group product items by product
            var productQuantities = productItems
                .GroupBy(pi => pi.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(pi => pi.Quantity));
            
            var result = new List<OutOfStockProductViewModel>();
            
            foreach (var product in products)
            {
                var totalQuantity = productQuantities.ContainsKey(product.ProductId) ? productQuantities[product.ProductId] : 0;
                
                if (totalQuantity <= lowStockThreshold)
                {
                    result.Add(new OutOfStockProductViewModel
                    {
                        ProductId = product.ProductId,
                        ProductName = CultureInfo.CurrentUICulture.Name == "ar-EG" ? product.NameAr : product.NameEn,
                        CurrentStock = totalQuantity,
                        LowStockThreshold = lowStockThreshold
                    });
                }
            }
            
            return result;
        }
        
        private Dictionary<DateTime, decimal> GetDailySales(List<Efreshli.Domain.Models.Order> orders)
        {
            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered);
            var dailySales = deliveredOrders
                .GroupBy(o => o.CreatedDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalPrice));
            
            // Fill in missing dates with zero values for the last 30 days
            var result = new Dictionary<DateTime, decimal>();
            var today = DateTime.Today;
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                result[date] = dailySales.ContainsKey(date) ? dailySales[date] : 0;
            }
            
            return result;
        }
        
        private Dictionary<DateTime, decimal> GetWeeklySales(List<Efreshli.Domain.Models.Order> orders)
        {
            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered);
            var weeklySales = deliveredOrders
                .GroupBy(o => GetWeekStartDate(o.CreatedDate))
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalPrice));
            
            // Fill in missing weeks with zero values for the last 12 weeks
            var result = new Dictionary<DateTime, decimal>();
            var today = DateTime.Today;
            var currentWeekStart = GetWeekStartDate(today);
            for (int i = 11; i >= 0; i--)
            {
                var weekStart = currentWeekStart.AddDays(-i * 7);
                result[weekStart] = weeklySales.ContainsKey(weekStart) ? weeklySales[weekStart] : 0;
            }
            
            return result;
        }
        
        private Dictionary<DateTime, decimal> GetMonthlySales(List<Efreshli.Domain.Models.Order> orders)
        {
            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered);
            var monthlySales = deliveredOrders
                .GroupBy(o => new DateTime(o.CreatedDate.Year, o.CreatedDate.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalPrice));
            
            // Fill in missing months with zero values for the last 12 months
            var result = new Dictionary<DateTime, decimal>();
            var today = DateTime.Today;
            var currentMonth = new DateTime(today.Year, today.Month, 1);
            for (int i = 11; i >= 0; i--)
            {
                var month = currentMonth.AddMonths(-i);
                result[month] = monthlySales.ContainsKey(month) ? monthlySales[month] : 0;
            }
            
            return result;
        }
        
        private SalesGrowthViewModel GetSalesGrowth(List<Efreshli.Domain.Models.Order> orders)
        {
            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered);
            var today = DateTime.Today;
            
            // Current week
            var currentWeekStart = GetWeekStartDate(today);
            var currentWeekEnd = currentWeekStart.AddDays(7);
            var currentWeekRevenue = deliveredOrders
                .Where(o => o.CreatedDate >= currentWeekStart && o.CreatedDate < currentWeekEnd)
                .Sum(o => o.TotalPrice);
            
            // Previous week
            var previousWeekStart = currentWeekStart.AddDays(-7);
            var previousWeekEnd = currentWeekStart;
            var previousWeekRevenue = deliveredOrders
                .Where(o => o.CreatedDate >= previousWeekStart && o.CreatedDate < previousWeekEnd)
                .Sum(o => o.TotalPrice);
            
            // Current month
            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);
            var currentMonthRevenue = deliveredOrders
                .Where(o => o.CreatedDate >= currentMonthStart && o.CreatedDate < nextMonthStart)
                .Sum(o => o.TotalPrice);
            
            // Previous month
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart;
            var previousMonthRevenue = deliveredOrders
                .Where(o => o.CreatedDate >= previousMonthStart && o.CreatedDate < previousMonthEnd)
                .Sum(o => o.TotalPrice);
            
            // Calculate growth
            var weeklyGrowthValue = currentWeekRevenue - previousWeekRevenue;
            var weeklyGrowthPercentage = previousWeekRevenue > 0 ? (weeklyGrowthValue / previousWeekRevenue) * 100 : 0;
            
            var monthlyGrowthValue = currentMonthRevenue - previousMonthRevenue;
            var monthlyGrowthPercentage = previousMonthRevenue > 0 ? (monthlyGrowthValue / previousMonthRevenue) * 100 : 0;
            
            return new SalesGrowthViewModel
            {
                WeeklyGrowthPercentage = (decimal)weeklyGrowthPercentage,
                WeeklyGrowthValue = weeklyGrowthValue,
                MonthlyGrowthPercentage = (decimal)monthlyGrowthPercentage,
                MonthlyGrowthValue = monthlyGrowthValue
            };
        }
        
        private List<RevenueTrendViewModel> GetWeeklyRevenueTrend(List<Efreshli.Domain.Models.Order> orders)
        {
            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered);
            var today = DateTime.Today;
            var currentWeekStart = GetWeekStartDate(today);
            
            var result = new List<RevenueTrendViewModel>();
            decimal previousWeekRevenue = 0;
            
            // Get data for the last 12 weeks
            for (int i = 11; i >= 0; i--)
            {
                var weekStart = currentWeekStart.AddDays(-i * 7);
                var weekEnd = weekStart.AddDays(7);
                
                var weekRevenue = deliveredOrders
                    .Where(o => o.CreatedDate >= weekStart && o.CreatedDate < weekEnd)
                    .Sum(o => o.TotalPrice);
                
                var growthPercentage = 0m;
                if (previousWeekRevenue > 0)
                {
                    growthPercentage = ((weekRevenue - previousWeekRevenue) / previousWeekRevenue) * 100;
                }
                
                result.Add(new RevenueTrendViewModel
                {
                    WeekStartDate = weekStart,
                    Revenue = weekRevenue,
                    GrowthPercentage = growthPercentage
                });
                
                previousWeekRevenue = weekRevenue;
            }
            
            return result;
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            // Validate the culture
            var supportedCultures = new[] { "en-US", "ar-EG" };
            if (!supportedCultures.Contains(culture))
            {
                culture = "ar-EG"; // Fallback to default culture
            }

            // Set the cookie
            Response.Cookies.Append(
                "LanguagePreference",
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.Lax,
                    Secure = true,
                    HttpOnly = true
                }
            );

            // Redirect to returnUrl if provided, otherwise to Home/Index
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
        }
    }
}