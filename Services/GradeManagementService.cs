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

        public async Task<GradeFormViewModel> EditGetGradeFormViewModelAsync(int id)
        {
            var grade = await data.Grades.FindAsync(id);
            if (grade == null)
            {
                return null;
            }

            var students = await data.Students.ToListAsync();
            var subjects = await data.Subjects.ToListAsync();

            var viewModel = new GradeFormViewModel
            {
                Id = grade.Id,
                SelectedSubjectId = grade.SubjectId,
                SelectedStudentId = grade.StudentId,
                Grade = grade.Value,
                StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList(),
                SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return viewModel;
        }

        public async Task<GradeViewModel> DeleteGetGradeViewModelAsync(int id)
        {
            var grade = await data.Grades
                .Include(g => g.Student)
                .Include(g => g.Teacher)
                .Include(g => g.Subject)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                return null;
            }

            var viewModel = new GradeViewModel
            {
                Id = grade.Id,
                Subject = grade.Subject.Name,
                StudentFirstName = grade.Student.FirstName,
                StudentLastName = grade.Student.LastName,
                TeacherFirstName = grade.Teacher.FirstName,
                TeacherLastName = grade.Teacher.LastName,
                Value = grade.Value,
                DateAndTime = grade.DateAndTime
            };

            return viewModel;
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

        public async Task<bool> UpdateGradeAsync(int id, GradeFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return false;
            }

            var grade = await data.Grades.FindAsync(id);
            if (grade == null)
            {
                return false;
            }

            grade.SubjectId = viewModel.SelectedSubjectId;
            grade.StudentId = viewModel.SelectedStudentId;
            grade.Value = viewModel.Grade;

            data.Update(grade);
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteGradeAsync(int id)
        {
            var grade = await data.Grades.FindAsync(id);
            if (grade == null)
            {
                return false;
            }

            data.Grades.Remove(grade);
            await data.SaveChangesAsync();

            return true;
        }
    }
}
