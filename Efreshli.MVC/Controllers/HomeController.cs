using Efreshli.MVC.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Efreshli.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            @ViewData["Title"] = "Dashboard";
            return View();
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
