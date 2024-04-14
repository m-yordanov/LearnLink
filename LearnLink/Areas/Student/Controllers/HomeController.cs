using LearnLink.Core.Models.Home;
using LearnLink.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearnLink.Areas.Student.Controllers
{
    public class HomeController : StudentBaseController
    {
        private readonly LearnLinkDbContext data;

        public HomeController(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IActionResult> Index()
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
    }
}
