using LearnLink.Core.Models;
using LearnLink.Core.Interfaces;
using static LearnLink.Core.Constants.MessageConstants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class GradeController : TeacherBaseController
    {
        private readonly IGradeManagementService gradeManagementService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeManagementService _gradeManagementService, IViewCommonService _viewCommonService)
        {
            gradeManagementService = _gradeManagementService;
            viewCommonService = _viewCommonService;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new GradeFormViewModel
            {
                StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList(),
                SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(GradeFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
                if (viewModel.SelectedStudentId <= 0)
                {
                    ViewData["SelectedStudentIdValidationError"] = "Please select a student.";
                }

                if (viewModel.SelectedSubjectId <= 0)
                {
                    ViewData["SelectedSubjectIdValidationError"] = "Please select a subject.";
                }

                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
				viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
				
                return View(viewModel);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await gradeManagementService.AddGradeAsync(viewModel, userId);

            if (!result)
            {
				TempData[UserMessageError] = "Failed to add the grade!";

				ModelState.AddModelError("", "Failed to add grade.");

                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                
                return View(viewModel);
            }

            TempData[UserMessageSuccess] = "You have added the grade!";
            return RedirectToAction(nameof(Add));
        }
    }
}
