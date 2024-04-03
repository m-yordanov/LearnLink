using LearnLink.Data;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class StudentController : Controller
{
    private readonly IGradeService _gradeService;
    private readonly IAttendanceService _attendanceService;

    public StudentController(IGradeService gradeService, IAttendanceService attendanceService)
    {
        _gradeService = gradeService;
        _attendanceService = attendanceService;
    }

    public async Task<IActionResult> AllGrades()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var studentGrades = await _gradeService.GetStudentGradesAsync(userId);
        return View(nameof(AllGrades), studentGrades);
    }

    public async Task<IActionResult> AllAttendances()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var attendances = await _attendanceService.GetStudentAttendancesAsync(studentId);
        return View(nameof(AllAttendances), attendances);
    }
}


