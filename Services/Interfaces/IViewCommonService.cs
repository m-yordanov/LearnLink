using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Services.Interfaces
{
    public interface IViewCommonService
    {
        Task<List<SelectListItem>> GetAvailableSubjectsAsync();

        Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync();

        Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync();
    }
}
