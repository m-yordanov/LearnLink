using LearnLink.Infrastructure.Data;
using LearnLink.Core.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class HomeController : TeacherBaseController
    {
        private readonly LearnLinkDbContext data;

        public HomeController(LearnLinkDbContext context)
        {
            data = context;
        }


        public async Task<IActionResult> Index()
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
    }
}
