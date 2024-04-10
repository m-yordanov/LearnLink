using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Services.Interfaces
{
    public interface IAttendanceManagementService
    {
        Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync();

        Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync();
        
        Task<bool> AddAttendanceAsync(AttendanceFormViewModel viewModel, string userId);

        Task<AttendanceFormViewModel> GetAttendanceForEditAsync(int? id);

        Task<bool> UpdateAttendanceAsync(int id, AttendanceFormViewModel viewModel);

        Task<AttendanceViewModel> GetAttendanceForDeleteAsync(int id);

        Task<bool> DeleteAttendanceAsync(int id);
    }
}
