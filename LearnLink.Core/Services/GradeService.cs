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

		public async Task<IEnumerable<GradeViewModel>> GetFilteredGradesAsync(GradeFilterModel filter)
		{
			return await ApplySorting(FilterGrades(filter), filter.SortBy, filter.SortDescending)
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
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

		public async Task<int> GetTotalFilteredGradesAsync(GradeFilterModel filter)
		{
			return await FilterGrades(filter).CountAsync();
		}

		public async Task<IEnumerable<GradeViewModel>> StudentGetFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
		{
			return await FilterStudentGrades(userId, selectedSubject, dateBefore, dateAfter)
				.OrderByDescending(g => g.DateAndTime)
				.ThenBy(g => g.Id)
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

		private static IQueryable<Grade> ApplySorting(IQueryable<Grade> query, string sortBy, bool descending)
		{
			IOrderedQueryable<Grade> ordered = (sortBy ?? string.Empty).ToLowerInvariant() switch
			{
				"student" => descending
					? query.OrderByDescending(g => g.Student.FirstName).ThenByDescending(g => g.Student.LastName)
					: query.OrderBy(g => g.Student.FirstName).ThenBy(g => g.Student.LastName),
				"teacher" => descending
					? query.OrderByDescending(g => g.Teacher.FirstName).ThenByDescending(g => g.Teacher.LastName)
					: query.OrderBy(g => g.Teacher.FirstName).ThenBy(g => g.Teacher.LastName),
				"subject" => descending
					? query.OrderByDescending(g => g.Subject.Name)
					: query.OrderBy(g => g.Subject.Name),
				"value" => descending
					? query.OrderByDescending(g => g.Value)
					: query.OrderBy(g => g.Value),
				_ => descending
					? query.OrderByDescending(g => g.DateAndTime)
					: query.OrderBy(g => g.DateAndTime)
			};

			return ordered.ThenBy(g => g.Id);
		}

		private IQueryable<Grade> FilterGrades(GradeFilterModel filter)
		{
			var query = data.Grades.AsQueryable();

			if (!string.IsNullOrEmpty(filter.SelectedStudent))
			{
				query = query.Where(g => (g.Student.FirstName + " " + g.Student.LastName).Contains(filter.SelectedStudent));
			}

			if (!string.IsNullOrEmpty(filter.SelectedTeacher))
			{
				query = query.Where(g => (g.Teacher.FirstName + " " + g.Teacher.LastName).Contains(filter.SelectedTeacher));
			}

			return ApplyCommonFilters(query, filter.SelectedSubject, filter.DateBefore, filter.DateAfter);
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
