using LearnLink.Data.Models;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
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

        public async Task<IActionResult> AllAttendances(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 1)
        {
            var attendancesViewModel = await attendanceService.GetFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredAttendances = await attendanceService.GetTotalFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = attendanceService.CalculateTotalPages(totalFilteredAttendances, pageSize);

            var attendances = attendancesViewModel.Select(a => new Attendance
            {
                Id = a.Id,
                Subject = new Subject { Name = a.Subject },
                Student = new Student { FirstName = a.StudentFirstName, LastName = a.StudentLastName },
                Teacher = new Teacher { FirstName = a.TeacherFirstName, LastName = a.TeacherLastName },
                Status = a.Status,
                DateAndTime = a.DateAndTime
            }).ToList();

            var viewModel = new AttendanceViewModel
            {
                FilteredAttendances = attendances,
                TotalCount = totalFilteredAttendances,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                SelectedStudent = selectedStudent,
                SelectedTeacher = selectedTeacher,
                SelectedSubject = selectedSubject,
                SelectedStatus = selectedStatus,
                SubjectOptions = await viewCommonService.GetAvailableSubjectsAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> EditAttendance(int? id)
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
        public async Task<IActionResult> EditAttendance(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();

                return View(viewModel);
            }

            var result = await attendanceManagementService.UpdateAttendanceAsync(id, viewModel);

            if (result)
            {
                return RedirectToAction(nameof(AllAttendances));
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> DeleteAttendance(int id)
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
            
            if (result)
            {
                return RedirectToAction(nameof(AllAttendances));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
