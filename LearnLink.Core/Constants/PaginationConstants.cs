namespace LearnLink.Core.Constants
{
    public static class PaginationConstants
    {
        public const int maxPerPage = 14;

        public const int maxPageSize = 100;

        public static int ClampPageNumber(int pageNumber)
            => pageNumber < 1 ? 1 : pageNumber;

        public static int ClampPageSize(int pageSize, int fallback = maxPerPage)
            => pageSize < 1 ? fallback : Math.Min(pageSize, maxPageSize);

        public static int ClampToLastPage(int pageNumber, int totalPages)
            => Math.Min(ClampPageNumber(pageNumber), Math.Max(totalPages, 1));
    }
}
