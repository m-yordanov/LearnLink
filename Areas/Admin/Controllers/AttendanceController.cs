using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using LearnLink.Services;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Areas.Admin.Controllers
{
    public class AttendanceController : AdminBaseController
    {
        private readonly LearnLinkDbContext data;
        private readonly IAttendanceService attendanceService;
        private readonly IAttendanceManagementService attendanceManagementService;

        public AttendanceController(LearnLinkDbContext context, IAttendanceService _attendanceService, IAttendanceManagementService _AttendanceManagementService)
        {
            data = context;
            attendanceService = _attendanceService;
            attendanceManagementService = _AttendanceManagementService;
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
                SelectedStatus = selectedStatus
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
                viewModel.StudentOptions = await data.Students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToListAsync();

                viewModel.SubjectOptions = await data.Subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToListAsync();

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
