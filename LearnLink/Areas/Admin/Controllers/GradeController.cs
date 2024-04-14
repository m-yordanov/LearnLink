using LearnLink.Core.Models;
using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
    public class GradeController : AdminBaseController
    {
        private readonly IGradeService gradeService;
        private readonly IGradeManagementService gradeManagementService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeService _gradeService, IGradeManagementService _gradeManagementService, IViewCommonService _viewCommonService)
        {
            gradeService = _gradeService;
            gradeManagementService = _gradeManagementService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 1)
        {
            var gradesViewModel = await gradeService.GetFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredGrades = await gradeService.GetTotalFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = gradeService.CalculateTotalPages(totalFilteredGrades, pageSize);

            var grades = gradeService.MapToGrades(gradesViewModel);

            var viewModel = new GradeViewModel
            {
                FilteredGrades = grades,
                TotalCount = totalFilteredGrades,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                SelectedStudent = selectedStudent,
                SelectedTeacher = selectedTeacher,
                SelectedSubject = selectedSubject,
                SubjectOptions = await viewCommonService.GetAvailableSubjectsAsync()
            };

            return View(viewModel);
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
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();

                return View(viewModel);
            }

            var success = await gradeManagementService.UpdateGradeAsync(id, viewModel);

            if (!success)
            {
                return NotFound();
            }

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

            return RedirectToAction(nameof(All));
        }
    }
}
