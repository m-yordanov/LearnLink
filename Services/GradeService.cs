using LearnLink.Data;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
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
}
