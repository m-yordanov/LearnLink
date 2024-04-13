using LearnLink.Data.Models;
using LearnLink.Data;
using LearnLink.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LearnLink.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly LearnLinkDbContext data;

        public SubjectService(LearnLinkDbContext _context)
        {
            data = _context;
        }

        public async Task<List<string>> GetAllSubjectsAsync()
        {
            return await data.Subjects
                .Select(s => s.Name)
                .ToListAsync();
        }


        public async Task<bool> CreateSubjectAsync(string subjectName)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                throw new ArgumentException("Subject name cannot be empty or null.");
            }

            var existingSubject = await data.Subjects.FirstOrDefaultAsync(s => s.Name == subjectName);
            if (existingSubject != null)
            {
                return false;
            }

            var newSubject = new Subject { Name = subjectName };
            data.Subjects.Add(newSubject);
            await data.SaveChangesAsync();

            return true; 
        }
    }
}
