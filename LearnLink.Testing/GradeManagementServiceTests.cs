using LearnLink.Core.Models;
using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Testing
{
    [TestFixture]
    public class GradeManagementServiceTests
    {
        private LearnLinkDbContext data = null!;
        private GradeManagementService service = null!;
        private string teacherUserId = null!;
        private int studentId;
        private int subjectId;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            service = new GradeManagementService(data);

            var teacherUser = data.AddUser("teacher@mail.com", "Viktor", "Georgiev");
            data.AddTeacher(teacherUser);
            teacherUserId = teacherUser.Id;

            studentId = data.AddStudent(data.AddUser("ivan@mail.com", "Ivan", "Petrov")).Id;
            subjectId = data.AddSubject("History").Id;
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        private GradeFormViewModel Form(int? student, int? subject) => new GradeFormViewModel
        {
            SelectedStudentId = student,
            SelectedSubjectId = subject,
            Grade = 5.5M
        };

        [Test]
        public async Task AddGradeAsync_AddsTheGradeForAValidSelection()
        {
            var added = await service.AddGradeAsync(Form(studentId, subjectId), teacherUserId);

            Assert.That(added, Is.True);
            Assert.That(data.Grades.Single().StudentId, Is.EqualTo(studentId));
        }

        [Test]
        public async Task AddGradeAsync_RejectsAStudentThatDoesNotExist()
        {
            var added = await service.AddGradeAsync(Form(studentId + 999, subjectId), teacherUserId);

            Assert.That(added, Is.False);
            Assert.That(data.Grades, Is.Empty);
        }

        [TestCase(null, TestName = "nothing selected")]
        [TestCase(0, TestName = "zero id")]
        [TestCase(-1, TestName = "negative id")]
        public async Task AddGradeAsync_RejectsAnUnselectedStudent(int? student)
        {
            var added = await service.AddGradeAsync(Form(student, subjectId), teacherUserId);

            Assert.That(added, Is.False);
            Assert.That(data.Grades, Is.Empty);
        }

        [Test]
        public async Task AddGradeAsync_RejectsASubjectThatDoesNotExist()
        {
            var added = await service.AddGradeAsync(Form(studentId, subjectId + 999), teacherUserId);

            Assert.That(added, Is.False);
            Assert.That(data.Grades, Is.Empty);
        }

        [Test]
        public async Task AddGradeAsync_RejectsAUserWhoIsNotATeacher()
        {
            var added = await service.AddGradeAsync(Form(studentId, subjectId), "not-a-teacher");

            Assert.That(added, Is.False);
            Assert.That(data.Grades, Is.Empty);
        }
    }
}
