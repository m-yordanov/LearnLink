using LearnLink.Core.Interfaces;
using LearnLink.Core.Models.Home;
using LearnLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Core.Services
{
    public class HomeService : IHomeService
    {
        private readonly LearnLinkDbContext data;

        public HomeService(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<StudentHomeViewModel> GetStudentHomeViewModelAsync(string userId)
        {
            var viewModel = new StudentHomeViewModel();
            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student != null)
            {
                var averageGrade = await data.Grades
                    .Where(g => g.Student.UserId == userId)
                    .AverageAsync(g => (decimal?)g.Value);

                var recentAttendances = await data.Attendances
                    .Include(a => a.Subject)
                    .Where(a => a.Student.UserId == userId)
                    .OrderByDescending(a => a.DateAndTime)
                    .Take(3)
                    .ToListAsync();

                viewModel.FirstName = student.FirstName;
                viewModel.Grade = averageGrade ?? 0;
                viewModel.Attendances = recentAttendances;
            }

            return viewModel;
        }

        public async Task<TeacherHomeViewModel> GetTeacherHomeViewModelAsync(string userId)
        {
            var viewModel = new TeacherHomeViewModel();
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher != null)
            {
                var recentlyAddedGrades = await data.Grades
                    .Include(g => g.Subject)
                    .Include(g => g.Student)
                    .Where(g => g.Teacher.UserId == userId)
                    .OrderByDescending(g => g.DateAndTime)
                    .Take(3)
                    .ToListAsync();

                var recentlyAddedAttendances = await data.Attendances
                    .Include(a => a.Subject)
                    .Include(a => a.Student)
                    .Where(a => a.Teacher.UserId == userId)
                    .OrderByDescending(a => a.DateAndTime)
                    .Take(3)
                    .ToListAsync();

                viewModel.FirstName = teacher.FirstName;
                viewModel.Grades = recentlyAddedGrades;
                viewModel.Attendances = recentlyAddedAttendances;
            }

            return viewModel;
        }
    }
}
