using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Core.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectViewModel>> GetFilteredSubjectsAsync(string searchString, int page, int pageSize);

        Task<int> GetTotalSubjectsCountAsync(string searchString);

        Task<bool> AddSubjectAsync(string subjectName);

        Task<bool> UpdateSubjectAsync(int id, string newSubjectName);

        Task<SubjectViewModel> EditSubjectFormViewModelAsync(int id);

        SubjectViewModel MapToSubjectViewModel(Subject subject);
    }
}
