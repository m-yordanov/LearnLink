using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Testing
{
    [TestFixture]
    public class GradeServiceTests
    {
        private LearnLinkDbContext data = null!;
        private GradeService service = null!;
        private Student ivan = null!;
        private Student maria = null!;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            service = new GradeService(data);

            ivan = data.AddStudent(data.AddUser("ivan@mail.com", "Ivan", "Petrov"));
            maria = data.AddStudent(data.AddUser("maria@mail.com", "Maria", "Ivanova"));

            var teacher = data.AddTeacher(data.AddUser("teacher@mail.com", "Viktor", "Georgiev"));
            var history = data.AddSubject("History");
            var geography = data.AddSubject("Geography");

            data.Grades.AddRange(
                NewGrade(ivan, teacher, history, 5.5M),
                NewGrade(ivan, teacher, geography, 4.0M),
                NewGrade(maria, teacher, history, 6.0M));
            data.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        private static Grade NewGrade(Student student, Teacher teacher, Subject subject, decimal value)
            => new Grade
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                Value = value,
                DateAndTime = new DateTime(2026, 5, 1, 9, 0, 0)
            };

        [Test]
        public async Task GetFilteredGradesAsync_AndTheCount_ApplyTheSameStudentFilter()
        {
            var page = await service.GetFilteredGradesAsync("Ivan Petrov", string.Empty, string.Empty, null, null, 1, 50);
            var total = await service.GetTotalFilteredGradesAsync("Ivan Petrov", string.Empty, string.Empty, null, null);

            Assert.That(page.Count(), Is.EqualTo(2));
            Assert.That(total, Is.EqualTo(page.Count()));
        }

        [Test]
        public async Task GetFilteredGradesAsync_AndTheCount_ApplyTheSameSubjectFilter()
        {
            var page = await service.GetFilteredGradesAsync(string.Empty, string.Empty, "History", null, null, 1, 50);
            var total = await service.GetTotalFilteredGradesAsync(string.Empty, string.Empty, "History", null, null);

            Assert.That(page.Count(), Is.EqualTo(2));
            Assert.That(total, Is.EqualTo(page.Count()));
        }

        [Test]
        public async Task StudentGetFilteredGradesAsync_ReturnsOnlyThatStudentsGrades()
        {
            var page = await service.StudentGetFilteredGradesAsync(ivan.UserId, string.Empty, null, null, 1, 50);

            Assert.That(page.Count(), Is.EqualTo(2));
            Assert.That(page.All(g => g.Value != 6.0M), Is.True, "Maria's grade must not be visible to Ivan");
        }

        [Test]
        public async Task StudentGetTotalFilteredGradesAsync_CountsOnlyThatStudentsGrades()
        {
            var total = await service.StudentGetTotalFilteredGradesAsync(maria.UserId, string.Empty, null, null);

            Assert.That(total, Is.EqualTo(1));
        }

        [Test]
        public async Task StudentGetFilteredGradesAsync_ReturnsNewestFirst()
        {
            var teacher = data.Teachers.First();
            var subject = data.Subjects.First();

            var newer = NewGrade(ivan, teacher, subject, 3.0M);
            newer.DateAndTime = new DateTime(2026, 6, 1, 9, 0, 0);
            data.Grades.Add(newer);
            data.SaveChanges();

            var page = await service.StudentGetFilteredGradesAsync(ivan.UserId, string.Empty, null, null, 1, 50);

            Assert.That(page.First().DateAndTime, Is.EqualTo(new DateTime(2026, 6, 1, 9, 0, 0)));
        }
    }
}
