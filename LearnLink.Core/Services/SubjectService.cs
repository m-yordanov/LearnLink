using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace LearnLink.Core.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly LearnLinkDbContext data;

        public SubjectService(LearnLinkDbContext _context)
        {
            data = _context;
        }

        public async Task<List<SubjectViewModel>> GetFilteredSubjectsAsync(string searchString, int page, int pageSize)
        {
            var query = data.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Name.Contains(searchString));
            }

            var subjects = await query
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var subjectViewModels = subjects.Select(s => MapToSubjectViewModel(s)).ToList();

            return subjectViewModels;
        }

        public async Task<int> GetTotalSubjectsCountAsync(string searchString)
        {
            IQueryable<Subject> query = data.Subjects;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Name.Contains(searchString));
            }

            var totalSubjectsCount = await query.CountAsync();

            return totalSubjectsCount;
        }
        public SubjectViewModel MapToSubjectViewModel(Subject subject)
        {
            return new SubjectViewModel
            {
                Id = subject.Id,
                SubjectName = subject.Name,
            };
        }

        public async Task<bool> AddSubjectAsync(string subjectName)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                throw new ArgumentException("Subject name cannot be empty or null.", nameof(subjectName));
            }

            // Check if the subject already exists
            var existingSubject = await data.Subjects.FirstOrDefaultAsync(s => s.Name == subjectName);
            if (existingSubject != null)
            {
                return false; // Subject already exists, return false indicating failure
            }

            // Create a new Subject entity
            var newSubject = new Subject { Name = subjectName };

            // Add the new subject to the database
            data.Subjects.Add(newSubject);

            try
            {
                await data.SaveChangesAsync();
                return true; // Successfully added the subject, return true
            }
            catch (DbUpdateException)
            {
                // Handle any specific database exception (e.g., unique constraint violation)
                // Log the error if needed
                return false; // Return false indicating failure
            }
        }

        public async Task<bool> UpdateSubjectAsync(int id, string newSubjectName)
        {
            var subject = await data.Subjects.FindAsync(id);
            if (subject == null)
            {
                return false;
            }

            subject.Name = newSubjectName;

            try
            {
                await data.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<SubjectViewModel> EditSubjectFormViewModelAsync(int id)
        {
            var subject = await data.Subjects.FindAsync(id);
            if (subject == null)
            {
                return null;
            }

            var viewModel = new SubjectViewModel
            {
                SubjectName = subject.Name
            };

            return viewModel;
        }
    }
}
