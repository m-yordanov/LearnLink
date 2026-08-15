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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            pageSize = ClampPageSize(pageSize);

            var totalFilteredGrades = await gradeService.StudentGetTotalFilteredGradesAsync(userId, selectedSubject, dateBefore, dateAfter);

            int totalPages = viewCommonService.CalculateTotalPages(totalFilteredGrades, pageSize);

            pageNumber = ClampToLastPage(pageNumber, totalPages);

            var filteredGrades = await gradeService.StudentGetFilteredGradesAsync(userId, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);

            var subjectOptions = await viewCommonService.GetAvailableSubjectsAsync();

            var viewModel = new GradeViewModel
            {
                FilteredGrades = filteredGrades,
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
