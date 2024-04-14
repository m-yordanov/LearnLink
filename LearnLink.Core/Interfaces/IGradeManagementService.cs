using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
	public interface IGradeManagementService
    {
        Task<GradeFormViewModel> EditGetGradeFormViewModelAsync(int id);

        Task<GradeViewModel> DeleteGetGradeViewModelAsync(int id);

        Task<bool> AddGradeAsync(GradeFormViewModel viewModel, string userId);

        Task<bool> UpdateGradeAsync(int id, GradeFormViewModel viewModel);

        Task<bool> DeleteGradeAsync(int id);
    }
}
