namespace LearnLink.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<string>> GetAllSubjectsAsync();

        Task<bool> CreateSubjectAsync(string subjectName);
    }
}
