using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
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

    }
}
