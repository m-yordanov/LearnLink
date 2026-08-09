using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using LearnLink.Infrastructure.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Core.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly LearnLinkDbContext data;

        public AttendanceService(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IEnumerable<AttendanceViewModel>> GetFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
        {
            return await FilterAttendances(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AttendanceViewModel
                {
                    Id = a.Id,
                    Subject = a.Subject.Name,
                    StudentFirstName = a.Student.FirstName,
                    StudentLastName = a.Student.LastName,
                    Status = a.Status,
                    DateAndTime = a.DateAndTime,
                    TeacherFirstName = a.Teacher.FirstName,
                    TeacherLastName = a.Teacher.LastName,
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter)
        {
            return await FilterAttendances(selectedStudent, selectedTeacher, selectedSubject, selectedStatus, dateBefore, dateAfter)
                .CountAsync();
        }

        public async Task<IEnumerable<AttendanceViewModel>> StudentGetFilteredAttendancesAsync(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus, int pageNumber, int pageSize)
        {
            return await FilterStudentAttendances(userId, selectedSubject, dateAfter, dateBefore, selectedStatus)
                .OrderByDescending(a => a.DateAndTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AttendanceViewModel
                {
                    DateAndTime = a.DateAndTime,
                    Status = a.Status,
                    Subject = a.Subject.Name,
                    TeacherFirstName = a.Teacher.FirstName,
                    TeacherLastName = a.Teacher.LastName
                })
                .ToListAsync();
        }

        public async Task<int> StudentGetTotalFilteredAttendancesAsync(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus)
        {
            return await FilterStudentAttendances(userId, selectedSubject, dateAfter, dateBefore, selectedStatus)
                .CountAsync();
        }

        private IQueryable<Attendance> FilterAttendances(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter)
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

            if (dateBefore.HasValue)
            {
                query = query.Where(a => a.DateAndTime < dateBefore);
            }

            if (dateAfter.HasValue)
            {
                query = query.Where(a => a.DateAndTime > dateAfter);
            }

            return ApplySubjectAndStatusFilters(query, selectedSubject, selectedStatus);
        }

        private IQueryable<Attendance> FilterStudentAttendances(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus)
        {
            var query = data.Attendances.Where(a => a.Student.UserId == userId);

            if (dateAfter.HasValue)
            {
                query = query.Where(a => a.DateAndTime >= dateAfter);
            }

            if (dateBefore.HasValue)
            {
                query = query.Where(a => a.DateAndTime <= dateBefore);
            }

            return ApplySubjectAndStatusFilters(query, selectedSubject, selectedStatus);
        }

        private static IQueryable<Attendance> ApplySubjectAndStatusFilters(IQueryable<Attendance> query, string selectedSubject, string selectedStatus)
        {
            if (!string.IsNullOrEmpty(selectedSubject))
            {
                query = query.Where(a => a.Subject.Name == selectedSubject);
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                if (!Enum.TryParse<AttendanceStatus>(selectedStatus, ignoreCase: true, out var statusEnum))
                {
                    throw new ArgumentException("Invalid attendance status provided.");
                }

                query = query.Where(a => a.Status == statusEnum);
            }

            return query;
        }

        public IEnumerable<Attendance> MapToAttendances(IEnumerable<AttendanceViewModel> attendancesViewModel)
        {
            return attendancesViewModel.Select(a => new Attendance
            {
                Id = a.Id,
                Subject = new Subject { Name = a.Subject },
                Student = new Student { FirstName = a.StudentFirstName, LastName = a.StudentLastName },
                Teacher = new Teacher { FirstName = a.TeacherFirstName, LastName = a.TeacherLastName },
                Status = a.Status,
                DateAndTime = a.DateAndTime
            });
        }
    }
}
