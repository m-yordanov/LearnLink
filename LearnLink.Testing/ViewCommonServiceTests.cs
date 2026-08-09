using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;

namespace LearnLink.Testing
{
    [TestFixture]
    public class ViewCommonServiceTests
    {
        private LearnLinkDbContext data = null!;
        private ViewCommonService service = null!;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            service = new ViewCommonService(data);
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        [TestCase(0, 10, 0)]
        [TestCase(1, 10, 1)]
        [TestCase(10, 10, 1)]
        [TestCase(11, 10, 2)]
        [TestCase(29, 14, 3)]
        public void CalculateTotalPages_RoundsUp(int total, int pageSize, int expected)
        {
            Assert.That(service.CalculateTotalPages(total, pageSize), Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStudentOptionsAsync_ExcludesDeactivatedStudents()
        {
            data.AddStudent(data.AddUser("active@mail.com", "Ivan", "Petrov"));
            data.AddStudent(data.AddUser("gone@mail.com", "Maria", "Ivanova"), isActive: false);

            var options = await service.GetStudentOptionsAsync();

            Assert.That(options.Select(o => o.Text), Is.EqualTo(new[] { "Ivan Petrov" }));
        }

        [Test]
        public async Task GetSubjectOptionsAsync_ReturnsEverySubject()
        {
            data.AddSubject("History");
            data.AddSubject("Geography");

            var options = await service.GetSubjectOptionsAsync();

            Assert.That(options.Select(o => o.Text), Is.EquivalentTo(new[] { "History", "Geography" }));
        }
    }
}
