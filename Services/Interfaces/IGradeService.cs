using LearnLink.Data.Models;
using LearnLink.Models;

namespace LearnLink.Services.Interfaces
{
    public interface IGradeService
    {
        Task<IEnumerable<GradeViewModel>> GetStudentGradesAsync(string userId);

        Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize);

        Task<int> GetTotalFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter);

        int CalculateTotalPages(int totalFilteredAttendances, int pageSize);

        public IEnumerable<Grade> MapToGrades(IEnumerable<GradeViewModel> gradesViewModel);
    }
}
