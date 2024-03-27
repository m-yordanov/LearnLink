using LearnLink.Data;
using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class StudentController : Controller
{
    private readonly LearnLinkDbContext data;

    public StudentController(LearnLinkDbContext context)
    {
        data = context;
    }

    public async Task<IActionResult> AllGrades()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var studentGrades = await data.Grades
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .Where(g => g.Student.UserId == userId)
            .Select(g => new GradeViewModel
            {
                Subject = g.Subject.Name,
                Value = g.Value,
                DateAndTime = g.DateAndTime,
                TeacherFirstName = g.Teacher.FirstName,
                TeacherLastName = g.Teacher.LastName
            })
            .ToListAsync();

        return View(nameof(AllGrades), studentGrades);
    }

    public async Task<IActionResult> AllAttendances()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var attendances = await data.Attendances
            .Include(a => a.Subject)
            .Include(a => a.Teacher)
            .Where(a => a.Student.UserId == studentId)
            .ToListAsync();

        var attendanceViewModels = attendances.Select(a => new AttendanceViewModel
        {
            DateAndTime = a.DateAndTime,
            Status = a.Status,
            Subject = a.Subject.Name,
            TeacherFirstName = a.Teacher.FirstName,
            TeacherLastName = a.Teacher.LastName
        }).ToList();

        return View(nameof(AllAttendances), attendanceViewModels);
    }
}

