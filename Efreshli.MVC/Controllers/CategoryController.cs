using Microsoft.AspNetCore.Mvc;

namespace Efreshli.MVC.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Categories Management";
            return View("Main");
        }
        //public IActionResult Main()
        //{
        //    ViewData["Title"] = "Create Management";
        //    return View();
        //}
    }
}
