using LearnLink.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Services.Interfaces
{
    public interface IGradeManagementService
    {
        Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync();

        Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync();

        Task<GradeFormViewModel> EditGetGradeFormViewModelAsync(int id);

        Task<GradeViewModel> DeleteGetGradeViewModelAsync(int id);

        Task<bool> AddGradeAsync(GradeFormViewModel viewModel, string userId);

        Task<bool> UpdateGradeAsync(int id, GradeFormViewModel viewModel);

        Task<bool> DeleteGradeAsync(int id);
    }
}
