using LearnLink.Data;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class GradeService : IGradeService
{
    private readonly LearnLinkDbContext data;

    public GradeService(LearnLinkDbContext context)
    {
        data = context;
    }

    public async Task<IEnumerable<GradeViewModel>> GetStudentGradesAsync(string userId)
    {
        return await data.Grades
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .Where(g => g.Student.UserId == userId)
            .Select(g => new GradeViewModel
            {
                Subject = g.Subject.Name,
                Value = g.Value,
                DateAndTime = g.DateAndTime,
                TeacherFirstName = g.Teacher.FirstName,
                TeacherLastName = g.Teacher.LastName
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
    {
        var query = data.Grades
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Include(g => g.Teacher)
            .Select(g => new GradeViewModel
            {
                Id = g.Id,
                Subject = g.Subject.Name,
                StudentFirstName = g.Student.FirstName,
                StudentLastName = g.Student.LastName,
                Value = g.Value,
                DateAndTime = g.DateAndTime,
                TeacherFirstName = g.Teacher.FirstName,
                TeacherLastName = g.Teacher.LastName,
            });

        if (!string.IsNullOrEmpty(selectedStudent))
        {
            query = query.Where(g => (g.StudentFirstName + " " + g.StudentLastName).Contains(selectedStudent));
        }

        if (!string.IsNullOrEmpty(selectedTeacher))
        {
            query = query.Where(g => (g.TeacherFirstName + " " + g.TeacherLastName).Contains(selectedTeacher));
        }

        if (!string.IsNullOrEmpty(selectedSubject))
        {
            query = query.Where(g => g.Subject == selectedSubject);
        }

        if (dateBefore != null)
        {
            query = query.Where(g => g.DateAndTime < dateBefore);
        }

        if (dateAfter != null)
        {
            query = query.Where(g => g.DateAndTime > dateAfter);
        }

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> GetTotalFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
    {
        var query = data.Grades.AsQueryable();

        if (!string.IsNullOrEmpty(selectedStudent))
        {
            query = query.Where(g => (g.Student.FirstName + " " + g.Student.LastName).Contains(selectedStudent));
        }

        if (!string.IsNullOrEmpty(selectedTeacher))
        {
            query = query.Where(g => (g.Teacher.FirstName + " " + g.Teacher.LastName).Contains(selectedTeacher));
        }

        if (!string.IsNullOrEmpty(selectedSubject))
        {
            query = query.Where(g => g.Subject.Name == selectedSubject);
        }

        if (dateBefore != null)
        {
            query = query.Where(g => g.DateAndTime < dateBefore);
        }

        if (dateAfter != null)
        {
            query = query.Where(g => g.DateAndTime > dateAfter);
        }

        return await query.CountAsync();
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
}
