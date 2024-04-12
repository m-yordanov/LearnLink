using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class StudentController : Controller
{
    private readonly IGradeService gradeService;
    private readonly IAttendanceService attendanceService;

    public StudentController(IGradeService _gradeService, IAttendanceService _attendanceService)
    {
        gradeService = _gradeService;
        attendanceService = _attendanceService;
    }

    public async Task<IActionResult> AllGrades()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var studentGrades = await gradeService.GetStudentGradesAsync(userId);

        return View(nameof(AllGrades), studentGrades);
    }

    public async Task<IActionResult> AllAttendances()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var attendances = await attendanceService.GetStudentAttendancesAsync(studentId);
        
        return View(nameof(AllAttendances), attendances);
    }
}


