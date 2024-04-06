using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Areas.Admin.Controllers
{
    public class GradeController : AdminBaseController
    {
        private readonly LearnLinkDbContext data;
        private readonly IGradeService gradeService;
        private readonly IGradeManagementService gradeManagementService;

        public GradeController(LearnLinkDbContext context, IGradeService _gradeService, IGradeManagementService _gradeManagementService)
        {
            data = context;
            gradeService = _gradeService;
            gradeManagementService = _gradeManagementService;
        }


        public async Task<IActionResult> AllGrades(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 8)
        {
            var gradesViewModel = await gradeService.GetFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredGrades = await gradeService.GetTotalFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = (int)Math.Ceiling((double)totalFilteredGrades / pageSize);

            var grades = gradesViewModel.Select(g => new Grade
            {
                Id = g.Id,
                Subject = new Subject { Name = g.Subject },
                Student = new Student { FirstName = g.StudentFirstName, LastName = g.StudentLastName },
                Teacher = new Teacher { FirstName = g.TeacherFirstName, LastName = g.TeacherLastName },
                Value = g.Value,
                DateAndTime = g.DateAndTime
            });

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

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await gradeService.EditGetGradeFormViewModelAsync(id.Value);

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
            var viewModel = await gradeService.DeleteGetGradeViewModelAsync(id);

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
