using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Core.Services
{
    public class AttendanceListService : IAttendanceListService
    {
        private readonly IAttendanceService attendanceService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceListService(IAttendanceService _attendanceService, IViewCommonService _viewCommonService)
        {
            attendanceService = _attendanceService;
            viewCommonService = _viewCommonService;
        }

        public async Task<AttendanceViewModel> BuildAsync(AttendanceFilterModel filter)
        {
            var totalCount = await attendanceService.GetTotalFilteredAttendancesAsync(filter);
            var totalPages = viewCommonService.CalculateTotalPages(totalCount, filter.PageSize);

            filter.PageNumber = ClampToLastPage(filter.PageNumber, totalPages);

            var attendances = await attendanceService.GetFilteredAttendancesAsync(filter);

            return new AttendanceViewModel
            {
                FilteredAttendances = attendances,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                SelectedStudent = filter.SelectedStudent,
                SelectedTeacher = filter.SelectedTeacher,
                SelectedSubject = filter.SelectedSubject,
                SelectedStatus = filter.SelectedStatus,
                DateBefore = filter.DateBefore ?? DateTime.MinValue,
                DateAfter = filter.DateAfter ?? DateTime.MinValue,
                SortBy = filter.SortBy,
                SortDescending = filter.SortDescending,
                SubjectOptions = await viewCommonService.GetAvailableSubjectsAsync()
            };
        }
    }
}
