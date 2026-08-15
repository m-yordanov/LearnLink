using LearnLink.Core.Models;
using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Testing
{
    [TestFixture]
    public class GradeListServiceTests
    {
        private LearnLinkDbContext data = null!;
        private GradeListService service = null!;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            service = new GradeListService(new GradeService(data), new ViewCommonService(data));

            var student = data.AddStudent(data.AddUser("ivan@mail.com", "Ivan", "Petrov"));
            var teacher = data.AddTeacher(data.AddUser("teacher@mail.com", "Viktor", "Georgiev"));
            var history = data.AddSubject("History");

            for (var i = 0; i < 5; i++)
            {
                data.Grades.Add(new Grade
                {
                    StudentId = student.Id,
                    TeacherId = teacher.Id,
                    SubjectId = history.Id,
                    Value = 3.0M + i,
                    DateAndTime = new DateTime(2026, 5, 1, 9, 0, 0).AddDays(i)
                });
            }

            data.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        private static GradeFilterModel Filter(int page, int size = 2)
            => new GradeFilterModel { PageNumber = page, PageSize = size };

        [Test]
        public async Task BuildAsync_ReturnsTheLastPageWhenAskedForOnePastTheEnd()
        {
            var lastPage = await service.BuildAsync(Filter(page: 3));
            var beyondTheEnd = await service.BuildAsync(Filter(page: 99));

            Assert.Multiple(() =>
            {
                Assert.That(beyondTheEnd.FilteredGrades.Select(g => g.Id),
                    Is.EqualTo(lastPage.FilteredGrades.Select(g => g.Id)),
                    "a page past the end must show the last page rather than an empty table");
                Assert.That(beyondTheEnd.FilteredGrades, Is.Not.Empty);
            });
        }

        [Test]
        public async Task BuildAsync_ReportsThePageItActuallyShowed()
        {
            var beyondTheEnd = await service.BuildAsync(Filter(page: 99));

            Assert.That(beyondTheEnd.PageNumber, Is.EqualTo(3),
                "the pager must agree with the rows underneath it");
        }

        [Test]
        public async Task BuildAsync_LeavesAPageInsideTheRangeAlone()
        {
            var viewModel = await service.BuildAsync(Filter(page: 2));

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.PageNumber, Is.EqualTo(2));
                Assert.That(viewModel.FilteredGrades.Count(), Is.EqualTo(2));
                Assert.That(viewModel.TotalPages, Is.EqualTo(3));
                Assert.That(viewModel.TotalCount, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task BuildAsync_SurvivesAPageNumberOfZero()
        {
            var viewModel = await service.BuildAsync(Filter(page: 0));

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.PageNumber, Is.EqualTo(1));
                Assert.That(viewModel.FilteredGrades.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task BuildAsync_ReturnsAnEmptyFirstPageWhenNothingMatches()
        {
            var filter = new GradeFilterModel { PageNumber = 4, SelectedStudent = "Nobody At All" };

            var viewModel = await service.BuildAsync(filter);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.FilteredGrades, Is.Empty);
                Assert.That(viewModel.TotalCount, Is.EqualTo(0));
                Assert.That(viewModel.PageNumber, Is.EqualTo(1));
            });
        }
    }
}
