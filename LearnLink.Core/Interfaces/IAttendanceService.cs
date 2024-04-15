using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceViewModel>> GetStudentAttendancesAsync(string studentId);

        Task<IEnumerable<AttendanceViewModel>> GetFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);

        Task<int> GetTotalFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter);

        Task<IEnumerable<AttendanceViewModel>> StudentGetFilteredAttendancesAsync(string studentId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus, int pageNumber, int pageSize);

        Task<int> StudentGetTotalFilteredAttendancesAsync(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus);

        IEnumerable<Attendance> MapToAttendances(IEnumerable<AttendanceViewModel> attendancesViewModel);
    }
}
