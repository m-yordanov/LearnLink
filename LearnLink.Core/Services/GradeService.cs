﻿using LearnLink.Core.Interfaces;
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

		public async Task<IEnumerable<GradeViewModel>> GetStudentGradesAsync(string userId)
		{
            var attendances = await data.Grades
				.Include(a => a.Subject)
				.Include(a => a.Teacher)
				.Where(a => a.Student.UserId == userId)
				.ToListAsync();

            return await data.Grades
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

        public async Task<IEnumerable<GradeViewModel>> StudentGetFilteredGradesAsync(string userId, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber, int pageSize)
        {
            var query = data.Grades
                .Include(g => g.Subject)
                .Include(g => g.Teacher)
                .Where(g => g.Student.UserId == userId);

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

            return await query
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
            var query = data.Grades
                .Where(g => g.Student.UserId == userId);

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


		public IEnumerable<Grade> MapToGrades(IEnumerable<GradeViewModel> gradesViewModel)
		{
			return gradesViewModel.Select(g => new Grade
			{
				Id = g.Id,
				Subject = new Subject { Name = g.Subject },
				Student = new Student { FirstName = g.StudentFirstName, LastName = g.StudentLastName },
				Teacher = new Teacher { FirstName = g.TeacherFirstName, LastName = g.TeacherLastName },
				Value = g.Value,
				DateAndTime = g.DateAndTime
			});
		}
    }
}

