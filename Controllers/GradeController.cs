using LearnLink.Data;
using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class GradeController : Controller
{
    private readonly LearnLinkDbContext data;

    public GradeController(LearnLinkDbContext context)
    {
        data = context;
    }

    public async Task<IActionResult> All()
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

        return View(nameof(All), studentGrades);
    }
}

