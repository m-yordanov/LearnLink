using LearnLink.Data;
using LearnLink.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace LearnLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LearnLinkDbContext data;

        public HomeController(ILogger<HomeController> logger, LearnLinkDbContext context)
        {
            _logger = logger;
            data = context;
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
                    return RedirectToAction("Index", "Home", new { area = "Teacher" });
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
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
        public async Task<IActionResult> StudentHomeAsync()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == studentId);

            if (student != null)
            {
                var averageGrade = await data.Grades
                    .Where(g => g.Student.UserId == studentId)
                    .AverageAsync(g => (decimal?)g.Value);

                var recentAttendances = await data.Attendances
                    .Include(a => a.Subject)
                    .Where(a => a.Student.UserId == studentId)
                    .OrderByDescending(a => a.DateAndTime)
                    .Take(3)
                    .ToListAsync();

                var viewModel = new StudentHomeViewModel
                {
                    FirstName = student.FirstName,
                    Grade = averageGrade ?? 0,
                    Attendances = recentAttendances
                };

                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Privacy()
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
