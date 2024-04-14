using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Student.Controllers
{
    public class AttendanceController : StudentBaseController
    {
        private readonly IAttendanceService attendanceService;

        public AttendanceController(IAttendanceService _attendanceService)
        {
            attendanceService = _attendanceService;
        }

        public async Task<IActionResult> All()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var attendances = await attendanceService.GetStudentAttendancesAsync(studentId);

            return View(nameof(All), attendances);
        }
    }
}
