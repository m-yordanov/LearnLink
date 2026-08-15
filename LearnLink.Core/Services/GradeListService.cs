using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Core.Services
{
    public class GradeListService : IGradeListService
    {
        private readonly IGradeService gradeService;
        private readonly IViewCommonService viewCommonService;

        public GradeListService(IGradeService _gradeService, IViewCommonService _viewCommonService)
        {
            gradeService = _gradeService;
            viewCommonService = _viewCommonService;
        }

        public async Task<GradeViewModel> BuildAsync(GradeFilterModel filter)
        {
            var totalCount = await gradeService.GetTotalFilteredGradesAsync(filter);
            var totalPages = viewCommonService.CalculateTotalPages(totalCount, filter.PageSize);

            filter.PageNumber = ClampToLastPage(filter.PageNumber, totalPages);

            var grades = await gradeService.GetFilteredGradesAsync(filter);

            return new GradeViewModel
            {
                FilteredGrades = grades,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                SelectedStudent = filter.SelectedStudent,
                SelectedTeacher = filter.SelectedTeacher,
                SelectedSubject = filter.SelectedSubject,
                DateBefore = filter.DateBefore ?? DateTime.MinValue,
                DateAfter = filter.DateAfter ?? DateTime.MinValue,
                SortBy = filter.SortBy,
                SortDescending = filter.SortDescending,
                SubjectOptions = await viewCommonService.GetAvailableSubjectsAsync()
            };
        }
    }
}
