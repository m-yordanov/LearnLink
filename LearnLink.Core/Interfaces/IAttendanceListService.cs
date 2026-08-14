using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IAttendanceListService
    {
        Task<AttendanceViewModel> BuildAsync(AttendanceFilterModel filter);
    }
}
