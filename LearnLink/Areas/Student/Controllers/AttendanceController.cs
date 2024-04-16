using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace LearnLink.Areas.Student.Controllers
{
    public class AttendanceController : StudentBaseController
    {
        private readonly IAttendanceService attendanceService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceService _attendanceService, IViewCommonService _viewCommonService)
        {
            attendanceService = _attendanceService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus, int pageNumber = 1, int pageSize = 10)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var filteredAttendances = await attendanceService.StudentGetFilteredAttendancesAsync(studentId, selectedSubject, dateAfter, dateBefore, selectedStatus, pageNumber, pageSize);

            var totalFilteredAttendances = await attendanceService.StudentGetTotalFilteredAttendancesAsync(studentId, selectedSubject, dateAfter, dateBefore, selectedStatus);

			int totalPages = viewCommonService.CalculateTotalPages(totalFilteredAttendances, pageSize);

			var subjectOptions = await viewCommonService.GetSubjectOptionsAsync();

            var statusOptions = Enum.GetValues(typeof(AttendanceStatus))
                         .Cast<AttendanceStatus>()
                         .Select(s => new SelectListItem
                         {
                             Value = s.ToString(),
                             Text = s.ToString()
                         });

            var attendances = attendanceService.MapToAttendances(filteredAttendances);

            var viewModel = new AttendanceViewModel
            {
                FilteredAttendances = attendances,
                SelectedSubject = selectedSubject,
                SelectedStatus = selectedStatus,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalFilteredAttendances,
                SubjectOptions = subjectOptions,
                StatusOptions = statusOptions
            };

            return View(viewModel);
        }
    }
}

