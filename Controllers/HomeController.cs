using Microsoft.AspNetCore.Mvc;

namespace ShiftManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Employee");
                }
                else
                {
                    return RedirectToAction("MyShifts", "ShiftSchedule");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
