using LearnLink.Core.Models;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Testing
{
    [TestFixture]
    public class PaginationClampingTests
    {
        [TestCase(0, TestName = "zero")]
        [TestCase(-1, TestName = "negative")]
        [TestCase(int.MinValue, TestName = "int.MinValue")]
        public void APageNumberBelowOne_BecomesTheFirstPage(int requested)
        {
            var filter = new GradeFilterModel { PageNumber = requested };

            Assert.That(filter.PageNumber, Is.EqualTo(1));
        }

        [Test]
        public void APageSizeOfZero_FallsBackToTheDefault()
        {
            var filter = new GradeFilterModel { PageSize = 0 };

            Assert.That(filter.PageSize, Is.EqualTo(maxPerPage));
        }

        [Test]
        public void APageSizeAboveTheCeiling_IsCappedRatherThanReadingTheWholeTable()
        {
            var filter = new AttendanceFilterModel { PageSize = 100_000 };

            Assert.That(filter.PageSize, Is.EqualTo(maxPageSize));
        }

        [Test]
        public void AValidPageSize_IsLeftAlone()
        {
            var filter = new AttendanceFilterModel { PageSize = 25 };

            Assert.That(filter.PageSize, Is.EqualTo(25));
        }

        [Test]
        public void TheFiltersDefaultToTheFirstPage()
        {
            Assert.Multiple(() =>
            {
                Assert.That(new GradeFilterModel().PageNumber, Is.EqualTo(1));
                Assert.That(new GradeFilterModel().PageSize, Is.EqualTo(maxPerPage));
                Assert.That(new AttendanceFilterModel().PageNumber, Is.EqualTo(1));
                Assert.That(new AttendanceFilterModel().PageSize, Is.EqualTo(maxPerPage));
            });
        }

        [Test]
        public void ClampToLastPage_PullsAPagePastTheEndBackToTheLastOne()
        {
            Assert.That(ClampToLastPage(999, 3), Is.EqualTo(3));
        }

        [Test]
        public void ClampToLastPage_LeavesAPageInsideTheRangeAlone()
        {
            Assert.That(ClampToLastPage(2, 3), Is.EqualTo(2));
        }

        [Test]
        public void ClampToLastPage_StillReturnsAValidPageWhenThereIsNoData()
        {
            Assert.That(ClampToLastPage(5, 0), Is.EqualTo(1));
        }
    }
}
