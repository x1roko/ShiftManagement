using Microsoft.AspNetCore.Mvc;

namespace ShiftManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Employee");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
