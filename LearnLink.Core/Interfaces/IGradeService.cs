using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
	public interface IGradeService
    {
        Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(GradeFilterModel filter);

        Task<int> GetTotalFilteredGradesAsync(GradeFilterModel filter);

        Task<IEnumerable<GradeViewModel>> StudentGetFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);


        Task<int> StudentGetTotalFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter);
    }
}
