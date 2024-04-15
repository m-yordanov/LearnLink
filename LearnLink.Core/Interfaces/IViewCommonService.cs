using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Core.Interfaces
{
    public interface IViewCommonService
    {
        Task<List<SelectListItem>> GetAvailableSubjectsAsync();

        Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync();

        Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync();

        int CalculateTotalPages(int entityTotal, int pageSize);
    }
}
