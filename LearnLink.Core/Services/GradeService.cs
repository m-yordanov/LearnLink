using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Core.Services
{
	public class GradeService : IGradeService
	{
		private readonly LearnLinkDbContext data;

		public GradeService(LearnLinkDbContext context)
		{
			data = context;
		}

		public async Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
		{
			return await FilterGrades(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
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
				})
				.ToListAsync();
		}

		public async Task<int> GetTotalFilteredGradesAsync(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
		{
			return await FilterGrades(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter)
				.CountAsync();
		}

		public async Task<IEnumerable<GradeViewModel>> StudentGetFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
		{
			return await FilterStudentGrades(userId, selectedSubject, dateBefore, dateAfter)
				.OrderByDescending(g => g.DateAndTime)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
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

		public async Task<int> StudentGetTotalFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
		{
			return await FilterStudentGrades(userId, selectedSubject, dateBefore, dateAfter)
				.CountAsync();
		}

		private IQueryable<Grade> FilterGrades(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
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

			return ApplyCommonFilters(query, selectedSubject, dateBefore, dateAfter);
		}

		private IQueryable<Grade> FilterStudentGrades(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
		{
			var query = data.Grades.Where(g => g.Student.UserId == userId);

			return ApplyCommonFilters(query, selectedSubject, dateBefore, dateAfter);
		}

		private static IQueryable<Grade> ApplyCommonFilters(IQueryable<Grade> query, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter)
		{
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

			return query;
		}
    }
}
