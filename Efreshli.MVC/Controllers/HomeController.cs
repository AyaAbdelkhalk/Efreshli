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
            
            // Get order status data
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            model.OrderStatusData = orders
                .GroupBy(o => o.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Get payment method data
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            model.PaymentMethodData = payments
                .GroupBy(p => p.PaymentMethod.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Get category product count
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            var products = await _unitOfWork.ProductRepository.GetAllAsync();
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
            
            // Get summary statistics
            model.TotalProducts = products.Count();
            model.TotalOrders = orders.Count();
            model.TotalUsers = (await _unitOfWork.UserRepository.GetAllAsync()).Count();
            model.TotalRevenue = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalPrice);
            
            return model;
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