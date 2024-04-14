using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Student.Controllers
{
    public class GradeController : StudentBaseController
    {
        private readonly IGradeService gradeService;

        public GradeController(IGradeService _gradeService)
        {
            gradeService = _gradeService;
        }

        public async Task<IActionResult> All()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentGrades = await gradeService.GetStudentGradesAsync(userId);

            return View(nameof(All), studentGrades);
        }
    }
}
