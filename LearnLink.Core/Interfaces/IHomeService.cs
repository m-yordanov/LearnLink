using LearnLink.Core.Models.Home;

namespace LearnLink.Core.Interfaces
{
    public interface IHomeService
    {
        Task<StudentHomeViewModel> GetStudentHomeViewModelAsync(string userId);

        Task<TeacherHomeViewModel> GetTeacherHomeViewModelAsync(string userId);
    }
}
