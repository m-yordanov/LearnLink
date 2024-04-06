using LearnLink.Data;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
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

        public async Task<IActionResult> AllAttendances()
        {
            var attendances = await data.Attendances
                .Select(a => new AttendanceViewModel
                {
                    Id = a.Id,
                    Subject = a.Subject.Name,
                    StudentFirstName = a.Student.FirstName,
                    StudentLastName = a.Student.LastName,
                    Status = a.Status,
                    DateAndTime = a.DateAndTime,
                    TeacherFirstName = a.Teacher.FirstName,
                    TeacherLastName = a.Teacher.LastName
                })
                .ToListAsync();

            return View(attendances);
        }

        public async Task<IActionResult> EditAttendance(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await data.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            var students = await data.Students.ToListAsync();
            var subjects = await data.Subjects.ToListAsync();

            var viewModel = new AttendanceFormViewModel
            {
                SelectedStudentId = attendance.StudentId,
                SelectedSubjectId = attendance.SubjectId,
                Status = attendance.Status,
                DateAndTime = attendance.DateAndTime,
                StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList(),
                SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),
                StatusOptions = Enum.GetValues(typeof(AttendanceStatus))
                    .Cast<AttendanceStatus>()
                    .Select(status => new SelectListItem
                    {
                        Value = ((int)status).ToString(),
                        Text = status.ToString()
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendance(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var students = await data.Students.ToListAsync();
                var subjects = await data.Subjects.ToListAsync();

                viewModel.StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList();

                viewModel.SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

                viewModel.StatusOptions = Enum.GetValues(typeof(AttendanceStatus))
                    .Cast<AttendanceStatus>()
                    .Select(status => new SelectListItem
                    {
                        Value = ((int)status).ToString(),
                        Text = status.ToString()
                    }).ToList();

                return View(viewModel);
            }

            var attendance = await data.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            attendance.StudentId = viewModel.SelectedStudentId;
            attendance.SubjectId = viewModel.SelectedSubjectId;
            attendance.Status = viewModel.Status;

            data.Update(attendance);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(AllAttendances));
        }

        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await data.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            var viewModel = new AttendanceViewModel
            {
                Id = attendance.Id,
                StudentFirstName = attendance.Student.FirstName,
                StudentLastName = attendance.Student.LastName,
                TeacherFirstName = attendance.Teacher.FirstName,
                TeacherLastName = attendance.Teacher.LastName,
                Status = attendance.Status,
                DateAndTime = attendance.DateAndTime
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string entityType)
        {
            var attendance = await data.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }
            data.Attendances.Remove(attendance);
            await data.SaveChangesAsync();
            return RedirectToAction(nameof(AllAttendances));
        }
    }
}
