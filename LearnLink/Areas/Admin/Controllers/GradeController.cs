﻿using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;
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

        public async Task<IActionResult> All(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = maxPerPage)
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
