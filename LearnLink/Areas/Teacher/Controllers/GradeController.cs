using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class GradeController : TeacherBaseController
    {
        private readonly IGradeListService gradeListService;
        private readonly IGradeManagementService gradeManagementService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeManagementService _gradeManagementService, IViewCommonService _viewCommonService, IGradeListService _gradeListService)
        {
            gradeManagementService = _gradeManagementService;
            viewCommonService = _viewCommonService;
            gradeListService = _gradeListService;
        }

        public async Task<IActionResult> All(GradeFilterModel filter)
        {
            return View(await gradeListService.BuildAsync(filter));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(GradeFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
				viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
				
                return View(viewModel);
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
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
