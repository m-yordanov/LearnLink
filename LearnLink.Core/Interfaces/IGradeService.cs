using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Core.Interfaces
{
	public interface IGradeService
    {
        Task<IEnumerable<GradeViewModel>> GetStudentGradesAsync(string userId);

        Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);

        Task<int> GetTotalFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter);

        Task<IEnumerable<GradeViewModel>> StudentGetFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);


        Task<int> StudentGetTotalFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter);

        public IEnumerable<Grade> MapToGrades(IEnumerable<GradeViewModel> gradesViewModel);
    }
}
