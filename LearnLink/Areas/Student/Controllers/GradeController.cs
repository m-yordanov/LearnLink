using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.PaginationConstants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Student.Controllers
{
    public class GradeController : StudentBaseController
    {
        private readonly IGradeService gradeService;
        private readonly IViewCommonService viewCommonService;

        public GradeController(IGradeService _gradeService, IViewCommonService _viewCommonService)
        {
            gradeService = _gradeService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = maxPerPage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var filteredGrades = await gradeService.StudentGetFilteredGradesAsync(userId, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredGrades = await gradeService.StudentGetTotalFilteredGradesAsync(userId, selectedSubject, dateBefore, dateAfter);

            int totalPages = viewCommonService.CalculateTotalPages(totalFilteredGrades, pageSize);

            var subjectOptions = await viewCommonService.GetAvailableSubjectsAsync();

            var grades = gradeService.MapToGrades(filteredGrades);

            var viewModel = new GradeViewModel
            {
                FilteredGrades = grades,
                TotalCount = totalFilteredGrades,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                SelectedSubject = selectedSubject,
                SubjectOptions = subjectOptions
            };

            return View(viewModel);
        }
    }
}
