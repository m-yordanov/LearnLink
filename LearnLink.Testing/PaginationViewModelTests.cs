using LearnLink.Core.Models;

namespace LearnLink.Testing
{
    [TestFixture]
    public class PaginationViewModelTests
    {
        private static PaginationViewModel Pager(int pageNumber, int totalPages)
            => new PaginationViewModel { PageNumber = pageNumber, TotalPages = totalPages };

        [TestCase(1, 54, 1, 15)]
        [TestCase(15, 54, 1, 15)]
        [TestCase(16, 54, 16, 30)]
        [TestCase(30, 54, 16, 30)]
        [TestCase(31, 54, 31, 45)]
        [TestCase(54, 54, 46, 54)]
        public void Window_CoversFifteenPagesAtATime(int page, int totalPages, int expectedStart, int expectedEnd)
        {
            var pager = Pager(page, totalPages);

            Assert.Multiple(() =>
            {
                Assert.That(pager.WindowStart, Is.EqualTo(expectedStart));
                Assert.That(pager.WindowEnd, Is.EqualTo(expectedEnd));
                Assert.That(pager.WindowEnd - pager.WindowStart + 1,
                    Is.LessThanOrEqualTo(PaginationViewModel.PagesPerWindow));
            });
        }

        [Test]
        public void Window_StopsAtTheLastPage()
        {
            var pager = Pager(1, 5);

            Assert.Multiple(() =>
            {
                Assert.That(pager.WindowEnd, Is.EqualTo(5));
                Assert.That(pager.HasPreviousWindow, Is.False);
                Assert.That(pager.HasNextWindow, Is.False);
            });
        }

        [Test]
        public void Arrows_AppearOnlyWhenThereIsAnotherWindow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Pager(1, 54).HasPreviousWindow, Is.False);
                Assert.That(Pager(1, 54).HasNextWindow, Is.True);
                Assert.That(Pager(16, 54).HasPreviousWindow, Is.True);
                Assert.That(Pager(54, 54).HasNextWindow, Is.False);
            });
        }

        [Test]
        public void Arrows_MoveAWholeWindow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Pager(1, 54).NextWindowPage, Is.EqualTo(16));
                Assert.That(Pager(16, 54).NextWindowPage, Is.EqualTo(31));
                Assert.That(Pager(16, 54).PreviousWindowPage, Is.EqualTo(1));
                Assert.That(Pager(31, 54).PreviousWindowPage, Is.EqualTo(16));
            });
        }

        [Test]
        public void PreviousWindowPage_NeverGoesBelowOne()
        {
            Assert.That(Pager(3, 54).PreviousWindowPage, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void Window_TreatsAnOutOfRangePageAsTheFirst(int page)
        {
            var pager = Pager(page, 54);

            Assert.That(pager.WindowStart, Is.EqualTo(1));
        }

        [Test]
        public void Window_ClampsAPageBeyondTheLastOne()
        {
            var pager = Pager(999, 54);

            Assert.Multiple(() =>
            {
                Assert.That(pager.CurrentPage, Is.EqualTo(54));
                Assert.That(pager.WindowStart, Is.EqualTo(46));
                Assert.That(pager.HasNextPage, Is.False);
                Assert.That(pager.PreviousPage, Is.EqualTo(53));
            });
        }

        [Test]
        public void PreviousAndNext_AppearOnlyWhenThereIsAnAdjacentPage()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Pager(1, 54).HasPreviousPage, Is.False, "no Previous on the first page");
                Assert.That(Pager(1, 54).HasNextPage, Is.True);
                Assert.That(Pager(27, 54).HasPreviousPage, Is.True);
                Assert.That(Pager(27, 54).HasNextPage, Is.True);
                Assert.That(Pager(54, 54).HasPreviousPage, Is.True);
                Assert.That(Pager(54, 54).HasNextPage, Is.False, "no Next on the last page");
            });
        }

        [Test]
        public void PreviousAndNext_MoveASinglePage()
        {
            var pager = Pager(27, 54);

            Assert.Multiple(() =>
            {
                Assert.That(pager.PreviousPage, Is.EqualTo(26));
                Assert.That(pager.NextPage, Is.EqualTo(28));
            });
        }

        [Test]
        public void Next_CrossesIntoTheFollowingWindow()
        {
            var pager = Pager(15, 54);

            Assert.Multiple(() =>
            {
                Assert.That(pager.NextPage, Is.EqualTo(16));
                Assert.That(Pager(pager.NextPage, 54).WindowStart, Is.EqualTo(16));
            });
        }

        [Test]
        public void OnASinglePageList_NeitherPreviousNorNextIsShown()
        {
            var pager = Pager(1, 1);

            Assert.Multiple(() =>
            {
                Assert.That(pager.HasPreviousPage, Is.False);
                Assert.That(pager.HasNextPage, Is.False);
            });
        }

        [Test]
        public void EveryPageIsReachableByWalkingTheWindows()
        {
            const int totalPages = 54;
            var reachable = new HashSet<int>();
            var pager = Pager(1, totalPages);

            while (true)
            {
                for (var i = pager.WindowStart; i <= pager.WindowEnd; i++)
                {
                    reachable.Add(i);
                }

                if (!pager.HasNextWindow)
                {
                    break;
                }

                pager = Pager(pager.NextWindowPage, totalPages);
            }

            Assert.That(reachable, Is.EquivalentTo(Enumerable.Range(1, totalPages)));
        }
    }
}
