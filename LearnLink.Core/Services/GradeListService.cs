using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;

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
            var grades = await gradeService.GetFilteredGradesAsync(filter);
            var totalCount = await gradeService.GetTotalFilteredGradesAsync(filter);

            return new GradeViewModel
            {
                FilteredGrades = grades,
                TotalCount = totalCount,
                TotalPages = viewCommonService.CalculateTotalPages(totalCount, filter.PageSize),
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
