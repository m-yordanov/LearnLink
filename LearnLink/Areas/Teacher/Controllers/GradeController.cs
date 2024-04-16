using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class GradeController : TeacherBaseController
    {
        private readonly IGradeService gradeService;
        private readonly IGradeManagementService gradeManagementService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeManagementService _gradeManagementService, IViewCommonService _viewCommonService, IGradeService _gradeService)
        {
            gradeManagementService = _gradeManagementService;
            viewCommonService = _viewCommonService;
            gradeService = _gradeService;
        }

        public async Task<IActionResult> All(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 1)
        {
            var gradesViewModel = await gradeService.GetFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredGrades = await gradeService.GetTotalFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = viewCommonService.CalculateTotalPages(totalFilteredGrades, pageSize);

            var subjectOptions = await viewCommonService.GetAvailableSubjectsAsync();

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
                SubjectOptions = subjectOptions
            };

            return View(viewModel);
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
