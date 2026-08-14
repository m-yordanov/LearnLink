using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
    public class GradeController : AdminBaseController
    {
        private readonly IGradeListService gradeListService;
        private readonly IGradeManagementService gradeManagementService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeListService _gradeListService, IGradeManagementService _gradeManagementService, IViewCommonService _viewCommonService)
        {
            gradeListService = _gradeListService;
            gradeManagementService = _gradeManagementService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(GradeFilterModel filter)
        {
            return View(await gradeListService.BuildAsync(filter));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await gradeManagementService.EditGetGradeFormViewModelAsync(id.Value);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GradeFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
				TempData[UserMessageError] = "Failed to edit the grade!";
				viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();

                return View(viewModel);
            }

            var success = await gradeManagementService.UpdateGradeAsync(id, viewModel);

            if (!success)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = "You have edited the grade!";

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await gradeManagementService.DeleteGetGradeViewModelAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await gradeManagementService.DeleteGradeAsync(id);

            if (!success)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = "You have deleted the grade!";
            return RedirectToAction(nameof(All));
        }
    }
}
