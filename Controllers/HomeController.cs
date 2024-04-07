using LearnLink.Data;
using LearnLink.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using LearnLink.Data.Constants;
using LearnLink.Data.Models;
using LearnLink.Models.Home;

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
                    return RedirectToAction("TeacherHome");
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

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherHome()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await data.Teachers.FirstOrDefaultAsync(s => s.UserId == teacherId);

            if (teacher != null)
            {
                var recentlyAddedGrades = await data.Grades
                    .Include(g => g.Subject)
                    .Include(g => g.Student)
                    .Where(g => g.Teacher.UserId == teacherId)
                    .OrderByDescending(g => g.DateAndTime)
                    .Take(3)
                    .ToListAsync();

                var recentlyAddedAttendances = await data.Attendances
                    .Include(a => a.Subject)
                    .Include(a => a.Student)
                    .Where(a => a.Teacher.UserId == teacherId)
                    .OrderByDescending(a => a.DateAndTime)
                    .Take(3) 
                    .ToListAsync();

                var viewModel = new TeacherHomeViewModel
                {
                    FirstName = teacher.FirstName,
                    Grades = recentlyAddedGrades,
                    Attendances = recentlyAddedAttendances
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
