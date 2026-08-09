using LearnLink.Core.Models;
using LearnLink.Core.Interfaces;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
    public class AttendanceController : AdminBaseController
    {
        private readonly IAttendanceService attendanceService;
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceService _attendanceService, IAttendanceManagementService _AttendanceManagementService, IViewCommonService _viewCommonService)
        {
            attendanceService = _attendanceService;
            attendanceManagementService = _AttendanceManagementService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, string sortBy = "date", bool sortDescending = true, int pageNumber = 1, int pageSize = maxPerPage)
        {
            var attendancesViewModel = await attendanceService.GetFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter, sortBy, sortDescending, pageNumber, pageSize);
            var totalFilteredAttendances = await attendanceService.GetTotalFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter);

            int totalPages = viewCommonService.CalculateTotalPages(totalFilteredAttendances, pageSize);

            var viewModel = new AttendanceViewModel
            {
                FilteredAttendances = attendancesViewModel,
                TotalCount = totalFilteredAttendances,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                SelectedStudent = selectedStudent,
                SelectedTeacher = selectedTeacher,
                SelectedSubject = selectedSubject,
                SelectedStatus = selectedStatus,
                SortBy = sortBy,
                SortDescending = sortDescending,
                SubjectOptions = await viewCommonService.GetAvailableSubjectsAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await attendanceManagementService.GetAttendanceForEditAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
				TempData[UserMessageError] = "Failed to edit the attendance!";
				viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();

                return View(viewModel);
            }

            var result = await attendanceManagementService.UpdateAttendanceAsync(id, viewModel);

            if (!result)
            {
                return BadRequest();
            }

            TempData[UserMessageSuccess] = "You have edited the attendance!";
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await attendanceManagementService.GetAttendanceForDeleteAsync(id);
            
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
            var result = await attendanceManagementService.DeleteAttendanceAsync(id);
            
            if (!result)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = "You have deleted the attendance!";
            return RedirectToAction(nameof(All));
        }
    }
}
