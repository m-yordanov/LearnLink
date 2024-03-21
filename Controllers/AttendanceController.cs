using LearnLink.Data;
using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly LearnLinkDbContext data;

        public AttendanceController(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IActionResult> All()
        {
            var attendances = await data.Attendances.ToListAsync();

            var attendanceViewModels = attendances.Select(a => new AttendanceViewModel
            {
                Id = a.Id,
                DateAndTime = a.DateAndTime,
                Status = a.Status,
                Subject = a.Subject.Name,
                TeacherFirstName = a.Teacher.FirstName,
                TeacherLastName = a.Teacher.LastName
            });

            return View(attendanceViewModels);
        }
    }
}
