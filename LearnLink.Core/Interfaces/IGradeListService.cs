using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IGradeListService
    {
        Task<GradeViewModel> BuildAsync(GradeFilterModel filter);
    }
}
