using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using LearnLink.Services;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Areas.Admin.Controllers
{
    public class GradeController : AdminBaseController
    {
        private readonly IGradeService gradeService;
        private readonly IGradeManagementService gradeManagementService;

        public GradeController(IGradeService _gradeService, IGradeManagementService _gradeManagementService)
        {
            gradeService = _gradeService;
            gradeManagementService = _gradeManagementService;
        }

        public async Task<IActionResult> AllGrades(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 1)
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
                SelectedSubject = selectedSubject
            };
            //var grades = gradesViewModel.Select(g => new Grade
            //{
            //    Id = g.Id,
            //    Subject = new Subject { Name = g.Subject },
            //    Student = new Student { FirstName = g.StudentFirstName, LastName = g.StudentLastName },
            //    Teacher = new Teacher { FirstName = g.TeacherFirstName, LastName = g.TeacherLastName },
            //    Value = g.Value,
            //    DateAndTime = g.DateAndTime
            //});

            //var viewModel = new GradeViewModel
            //{
            //    FilteredGrades = grades,
            //    TotalCount = totalFilteredGrades,
            //    PageSize = pageSize,
            //    PageNumber = pageNumber,
            //    TotalPages = totalPages,
            //    SelectedStudent = selectedStudent,
            //    SelectedTeacher = selectedTeacher,
            //    SelectedSubject = selectedSubject
            //};

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(int? id)
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
        public async Task<IActionResult> EditGrade(int id, GradeFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var success = await gradeManagementService.UpdateGradeAsync(id, viewModel);

            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(AllGrades));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGrade(int id)
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

            return RedirectToAction(nameof(AllGrades));
        }
    }
}
