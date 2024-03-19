using LearnLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class GradeController : Controller
{
    private readonly LearnLinkDbContext _context;

    public GradeController(LearnLinkDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var studentName = User.Identity.Name;
        var studentGrades = _context.Grades
            .Include(g => g.Subject)
            .Where(g => g.Student.IdentityUser.UserName == studentName)
            .OrderBy(g => g.Subject.Name)
            .ToList();

        return View(studentGrades);
    }
}
