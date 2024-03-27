using LearnLink.Models;
using LearnLink.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LearnLink.Controllers
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
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("StudentHome");
                }
                else if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction("TeacherHome");
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminHome");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View(); 
            }
        }

        [Authorize(Roles = "Student")]
        public IActionResult StudentHome()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult TeacherHome()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminHome()
        {
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
    }
}
