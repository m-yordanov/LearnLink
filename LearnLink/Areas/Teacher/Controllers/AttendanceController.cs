using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class AttendanceController : TeacherBaseController
    {
        private readonly IAttendanceService attendanceService;
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceManagementService _attendanceManagementService, IViewCommonService _viewCommonService, IAttendanceService _attendanceSerivce)
        {
            attendanceManagementService = _attendanceManagementService;
            viewCommonService = _viewCommonService;
            attendanceService = _attendanceSerivce;
        }
        public async Task<IActionResult> All(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = maxPerPage)
        {
            var attendancesViewModel = await attendanceService.GetFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredAttendances = await attendanceService.GetTotalFilteredAttendancesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = viewCommonService.CalculateTotalPages(totalFilteredAttendances, pageSize);

            var attendances = attendancesViewModel.Select(a => new Attendance
            {
                Id = a.Id,
                Subject = new Subject { Name = a.Subject },
                Student = new Infrastructure.Data.Models.Student { FirstName = a.StudentFirstName, LastName = a.StudentLastName },
                Teacher = new Infrastructure.Data.Models.Teacher { FirstName = a.TeacherFirstName, LastName = a.TeacherLastName },
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

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new AttendanceFormViewModel
            {
                StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList(),
                SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AttendanceFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
				if (viewModel.SelectedStudentId <= 0)
                {
                    ViewData["SelectedStudentIdValidationError"] = "Please select a student.";
                }

                if (viewModel.SelectedSubjectId <= 0)
                {
                    ViewData["SelectedSubjectIdValidationError"] = "Please select a subject.";
                }

                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
			}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await attendanceManagementService.AddAttendanceAsync(viewModel, userId);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to add attendance.");
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
            }

			TempData[UserMessageSuccess] = "You have added the attendance!";
			return RedirectToAction(nameof(Add));
        }
    }
}
