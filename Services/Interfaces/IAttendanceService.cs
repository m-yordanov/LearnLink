using LearnLink.Models;

namespace LearnLink.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceViewModel>> GetStudentAttendancesAsync(string studentId);
    }
}
