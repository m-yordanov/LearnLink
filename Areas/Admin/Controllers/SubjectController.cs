using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
    public class SubjectController : AdminBaseController
    {
        private readonly ISubjectService subjectService;

        public SubjectController(ISubjectService _subjectService)
        {
            subjectService = _subjectService;
        }

        public async Task<IActionResult> All()
        {
            var subjects = await subjectService.GetAllSubjectsAsync();

            return View(subjects);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string subjectName)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                ModelState.AddModelError("subjectName", "Please enter a subject name.");
                return RedirectToAction(nameof(All));
            }

            try
            {
                bool success = await subjectService.CreateSubjectAsync(subjectName);
                if (success)
                {
                    return RedirectToAction(nameof(All));
                }
                else
                {
                    ModelState.AddModelError("subjectName", "Subject already exists.");
                    return RedirectToAction(nameof(All));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create subject: {ex.Message}");
                return RedirectToAction(nameof(All));
            }
        }

    }
}
