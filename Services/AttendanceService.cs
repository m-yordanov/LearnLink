using LearnLink.Data;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Services
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
    }
}
