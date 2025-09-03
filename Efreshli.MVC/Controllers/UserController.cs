using Microsoft.AspNetCore.Mvc;

namespace Efreshli.MVC.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
