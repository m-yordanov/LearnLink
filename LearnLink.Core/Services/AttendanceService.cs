using LearnLink.Core.Models;
using Microsoft.EntityFrameworkCore;
using LearnLink.Core.Interfaces;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using LearnLink.Infrastructure.Data.Models.Enums;

namespace LearnLink.Core.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly LearnLinkDbContext data;

        public AttendanceService(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IEnumerable<AttendanceViewModel>> GetStudentAttendancesAsync(string studentId)
        {
            var attendances = await data.Attendances
                .Include(a => a.Subject)
                .Include(a => a.Teacher)
                .Where(a => a.Student.UserId == studentId)
                .ToListAsync();

            return attendances.Select(a => new AttendanceViewModel
            {
                DateAndTime = a.DateAndTime,
                Status = a.Status,
                Subject = a.Subject.Name,
                TeacherFirstName = a.Teacher.FirstName,
                TeacherLastName = a.Teacher.LastName
            });
        }

        public async Task<IEnumerable<AttendanceViewModel>> GetFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
        {
            IQueryable<Attendance> query = data.Attendances
                .Include(a => a.Subject)
                .Include(a => a.Student)
                .Include(a => a.Teacher);

            if (!string.IsNullOrEmpty(selectedStudent))
            {
                query = query.Where(a => (a.Student.FirstName + " " + a.Student.LastName).Contains(selectedStudent));
            }

            if (!string.IsNullOrEmpty(selectedTeacher))
            {
                query = query.Where(a => (a.Teacher.FirstName + " " + a.Teacher.LastName).Contains(selectedTeacher));
            }

            if (!string.IsNullOrEmpty(selectedSubject))
            {
                query = query.Where(a => a.Subject.Name == selectedSubject);
            }

            if (dateBefore.HasValue)
            {
                query = query.Where(a => a.DateAndTime < dateBefore);
            }

            if (dateAfter.HasValue)
            {
                query = query.Where(a => a.DateAndTime > dateAfter);
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                if (Enum.TryParse<AttendanceStatus>(selectedStatus, ignoreCase: true, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }
                else
                {
                    throw new ArgumentException("Invalid attendance status provided.");
                }
            }

            var totalFilteredAttendances = await query.CountAsync();

            var attendances = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return attendances.Select(a => new AttendanceViewModel
            {
                Id = a.Id,
                Subject = a.Subject.Name,
                StudentFirstName = a.Student.FirstName,
                StudentLastName = a.Student.LastName,
                Status = a.Status,
                DateAndTime = a.DateAndTime,
                TeacherFirstName = a.Teacher.FirstName,
                TeacherLastName = a.Teacher.LastName,
            });
        }


        public async Task<int> GetTotalFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
        {
            var query = data.Attendances.AsQueryable();

            if (!string.IsNullOrEmpty(selectedStudent))
            {
                query = query.Where(a => (a.Student.FirstName + " " + a.Student.LastName).Contains(selectedStudent));
            }

            if (!string.IsNullOrEmpty(selectedTeacher))
            {
                query = query.Where(a => (a.Teacher.FirstName + " " + a.Teacher.LastName).Contains(selectedTeacher));
            }

            if (!string.IsNullOrEmpty(selectedSubject))
            {
                query = query.Where(a => a.Subject.Name == selectedSubject);
            }

            if (dateBefore != null)
            {
                query = query.Where(a => a.DateAndTime < dateBefore);
            }

            if (dateAfter != null)
            {
                query = query.Where(a => a.DateAndTime > dateAfter);
            }

            return await query.CountAsync();
        }

        public int CalculateTotalPages(int totalFilteredAttendances, int pageSize)
        {
            return (int)Math.Ceiling((double)totalFilteredAttendances / pageSize);
        }
    }
}
