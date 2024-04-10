using LearnLink.Data;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Services
{
    public class ViewCommonService : IViewCommonService
    {
        private readonly LearnLinkDbContext data;

        public ViewCommonService(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<List<SelectListItem>> GetAvailableSubjectsAsync()
        {
            var subjects = await data.Subjects
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Name
                })
                .Distinct()
                .ToListAsync();

            return subjects;
        }

        public async Task<IEnumerable<SelectListItem>> GetStudentOptionsAsync()
        {
            var students = await data.Students.ToListAsync();
            return students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.FirstName} {s.LastName}"
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetSubjectOptionsAsync()
        {
            var subjects = await data.Subjects.ToListAsync();
            return subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            });
        }
    }
}
