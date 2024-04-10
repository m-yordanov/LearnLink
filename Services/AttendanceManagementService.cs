using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Services
{
    public class AttendanceManagementService : IAttendanceManagementService
    {
        private readonly LearnLinkDbContext data;

        public AttendanceManagementService(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync()
        {
            var students = await data.Students.ToListAsync();
            return students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.FirstName} {s.LastName}"
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync()
        {
            var subjects = await data.Subjects.ToListAsync();
            return subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            });
        }

        public async Task<bool> AddAttendanceAsync(AttendanceFormViewModel viewModel, string userId)
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
            {
                return false;
            }

            if (viewModel.SelectedStudentId <= 0 || viewModel.SelectedSubjectId <= 0)
            {
                return false;
            }

            var student = await data.Students.FirstOrDefaultAsync(s => s.Id == viewModel.SelectedStudentId);
            if (student == null)
            {
                return false;
            }

            var subject = await data.Subjects.FirstOrDefaultAsync(s => s.Id == viewModel.SelectedSubjectId);
            if (subject == null)
            {
                return false;
            }

            var attendance = new Attendance
            {
                StudentId = viewModel.SelectedStudentId,
                SubjectId = viewModel.SelectedSubjectId,
                Status = viewModel.Status,
                DateAndTime = viewModel.DateAndTime,
                TeacherId = teacher.Id
            };

            data.Attendances.Add(attendance);
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<AttendanceFormViewModel> GetAttendanceForEditAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var attendance = await data.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return null;
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
            };

            return viewModel;
        }

        public async Task<bool> UpdateAttendanceAsync(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return false;
            }

            var attendance = await data.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return false;
            }

            attendance.SubjectId = viewModel.SelectedSubjectId;
            attendance.StudentId = viewModel.SelectedStudentId;
            attendance.Status = viewModel.Status;
            attendance.DateAndTime = viewModel.DateAndTime;

            data.Update(attendance);
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<AttendanceViewModel> GetAttendanceForDeleteAsync(int id)
        {
            var attendance = await data.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return null;
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

            return viewModel;
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await data.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return false;
            }

            data.Attendances.Remove(attendance);
            await data.SaveChangesAsync();

            return true;
        }

        private bool AttendanceExists(int id)
        {
            return data.Attendances.Any(e => e.Id == id);
        }
    }
}
