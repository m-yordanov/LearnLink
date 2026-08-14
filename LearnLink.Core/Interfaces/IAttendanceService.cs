using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceViewModel>> GetFilteredAttendancesAsync(AttendanceFilterModel filter);

        Task<int> GetTotalFilteredAttendancesAsync(AttendanceFilterModel filter);

        Task<IEnumerable<AttendanceViewModel>> StudentGetFilteredAttendancesAsync(string studentId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus, int pageNumber, int pageSize);

        Task<int> StudentGetTotalFilteredAttendancesAsync(string userId, string selectedSubject, DateTime? dateAfter, DateTime? dateBefore, string selectedStatus);
    }
}
