using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using LearnLink.Infrastructure.Data.Models.Enums;

namespace LearnLink.Testing
{
    [TestFixture]
    public class AttendanceServiceTests
    {
        private LearnLinkDbContext data = null!;
        private AttendanceService service = null!;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            service = new AttendanceService(data);

            var student = data.AddStudent(data.AddUser("student@mail.com", "Ivan", "Petrov"));
            var teacher = data.AddTeacher(data.AddUser("teacher@mail.com", "Viktor", "Georgiev"));
            var subject = data.AddSubject("History");

            data.Attendances.AddRange(
                NewAttendance(student, teacher, subject, AttendanceStatus.Present),
                NewAttendance(student, teacher, subject, AttendanceStatus.Present),
                NewAttendance(student, teacher, subject, AttendanceStatus.Absent));
            data.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        private static Attendance NewAttendance(Student student, Teacher teacher, Subject subject, AttendanceStatus status)
            => new Attendance
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                Status = status,
                DateAndTime = new DateTime(2026, 5, 1, 9, 0, 0)
            };

        [Test]
        public async Task GetTotalFilteredAttendancesAsync_AppliesTheStatusFilter()
        {
            var total = await service.GetTotalFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, nameof(AttendanceStatus.Absent), null, null);

            Assert.That(total, Is.EqualTo(1));
        }

        [Test]
        public async Task GetTotalFilteredAttendancesAsync_AgreesWithTheFilteredPage()
        {
            var status = nameof(AttendanceStatus.Present);

            var page = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, status, null, null, "date", true, 1, 50);
            var total = await service.GetTotalFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, status, null, null);

            Assert.That(total, Is.EqualTo(page.Count()));
        }

        [Test]
        public async Task GetFilteredAttendancesAsync_ReturnsOnlyTheRequestedStatus()
        {
            var page = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, nameof(AttendanceStatus.Absent), null, null, "date", true, 1, 50);

            Assert.That(page.Select(a => a.Status), Is.EqualTo(new[] { AttendanceStatus.Absent }));
        }

        [Test]
        public async Task GetTotalFilteredAttendancesAsync_CountsEverythingWithoutFilters()
        {
            var total = await service.GetTotalFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, null, null);

            Assert.That(total, Is.EqualTo(3));
        }

        [TestCase("banana", TestName = "unparseable status")]
        [TestCase("99", TestName = "numeric status outside the enum")]
        public async Task AnUnrecognisedStatus_IsIgnoredRatherThanThrowing(string status)
        {
            var page = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, status, null, null, "date", true, 1, 50);
            var total = await service.GetTotalFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, status, null, null);

            Assert.Multiple(() =>
            {
                Assert.That(page.Count(), Is.EqualTo(3), "the filter is dropped, so every row is returned");
                Assert.That(total, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task AnUnrecognisedStatus_IsIgnoredForStudentsToo()
        {
            var userId = data.Students.First().UserId;

            var page = await service.StudentGetFilteredAttendancesAsync(
                userId, string.Empty, null, null, "banana", 1, 50);

            Assert.That(page.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task StatusMatchingIsCaseInsensitive()
        {
            var total = await service.GetTotalFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, "absent", null, null);

            Assert.That(total, Is.EqualTo(1));
        }

        [Test]
        public async Task StudentGetFilteredAttendancesAsync_ReturnsOnlyThatStudentsRecords()
        {
            var otherStudent = data.AddStudent(data.AddUser("other@mail.com", "Maria", "Ivanova"));
            var teacher = data.Teachers.First();
            var subject = data.Subjects.First();

            data.Attendances.Add(NewAttendance(otherStudent, teacher, subject, AttendanceStatus.Late));
            data.SaveChanges();

            var ownUserId = data.Students.First(s => s.Email == "student@mail.com").UserId;

            var page = await service.StudentGetFilteredAttendancesAsync(
                ownUserId, string.Empty, null, null, string.Empty, 1, 50);

            Assert.That(page.Count(), Is.EqualTo(3));
            Assert.That(page.Any(a => a.Status == AttendanceStatus.Late), Is.False);
        }

        [Test]
        public async Task GetFilteredAttendancesAsync_ReturnsNewestFirstByDefault()
        {
            var newest = NewAttendance(data.Students.First(), data.Teachers.First(), data.Subjects.First(), AttendanceStatus.Excused);
            newest.DateAndTime = new DateTime(2026, 9, 1, 9, 0, 0);
            data.Attendances.Add(newest);
            data.SaveChanges();

            var page = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, null, null, "date", true, 1, 50);

            Assert.That(page.First().Status, Is.EqualTo(AttendanceStatus.Excused));
        }

        [Test]
        public async Task GetFilteredAttendancesAsync_BreaksTiesById()
        {
            var page = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, null, null, "date", true, 1, 50);

            Assert.That(page.Select(a => a.Id), Is.Ordered.Ascending);
        }

        [Test]
        public async Task GetFilteredAttendancesAsync_FallsBackToTheDefaultForAnUnknownColumn()
        {
            var unknown = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, null, null, "banana", true, 1, 50);
            var byDate = await service.GetFilteredAttendancesAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, null, null, "date", true, 1, 50);

            Assert.That(unknown.Select(a => a.Id), Is.EqualTo(byDate.Select(a => a.Id)));
        }
    }
}
