using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
