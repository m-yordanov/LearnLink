using LearnLink.Core.Models.Home;
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
                    return RedirectToAction("Index", "Home", new { area = "Student" });
                }
                else if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Teacher" });
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction(nameof(Unassigned));
                }
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public IActionResult Unassigned()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            if (statusCode == 404)
            {
                return View("Error404");
            }

            return View("Error500", errorViewModel);
        }
    }
}
