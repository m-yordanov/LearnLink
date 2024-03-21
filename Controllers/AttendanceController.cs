using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
