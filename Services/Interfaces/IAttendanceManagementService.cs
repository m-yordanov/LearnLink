using LearnLink.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Services.Interfaces
{
    public interface IAttendanceManagementService
    {
        Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync();
        Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync();
        Task<bool> AddAttendanceAsync(AttendanceFormViewModel viewModel, string userId);
    }
}
