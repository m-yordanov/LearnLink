using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceViewModel>> GetFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);

        Task<int> GetTotalFilteredAttendancesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, string selectedStatus, DateTime? dateBefore, DateTime? dateAfter);

        Task<IEnumerable<AttendanceViewModel>> StudentGetFilteredAttendancesAsync(string studentId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus, int pageNumber, int pageSize);

        Task<int> StudentGetTotalFilteredAttendancesAsync(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus);
    }
}
