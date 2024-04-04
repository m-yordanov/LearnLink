using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Services
{
    public class GradeManagementService : IGradeManagementService
    {
        private readonly LearnLinkDbContext data;

        public GradeManagementService(LearnLinkDbContext context)
        {
            data = context;
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

        public async Task<bool> AddGradeAsync(GradeFormViewModel viewModel, string userId)
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return false;

            var subject = await data.Subjects.FirstOrDefaultAsync(s => s.Id == viewModel.SelectedSubjectId);
            if (subject == null)
                return false;


            var grade = new Grade
            {
                StudentId = viewModel.SelectedStudentId,
                Subject = subject,
                Value = viewModel.Grade,
                DateAndTime = DateTime.Now,
                TeacherId = teacher.Id
            };

            data.Grades.Add(grade);
            await data.SaveChangesAsync();

            return true;
        }
    }
}
