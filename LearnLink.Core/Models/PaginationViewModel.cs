namespace LearnLink.Core.Models
{
    public class PaginationViewModel
    {
        public const int PagesPerWindow = 15;

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public string PageParameter { get; set; } = "page";

        public Dictionary<string, string?> Filters { get; set; } = new Dictionary<string, string?>();

        public int CurrentPage => Math.Clamp(PageNumber, 1, Math.Max(TotalPages, 1));

        public int WindowStart => ((CurrentPage - 1) / PagesPerWindow) * PagesPerWindow + 1;

        public int WindowEnd => Math.Min(WindowStart + PagesPerWindow - 1, TotalPages);

        public bool HasPreviousWindow => WindowStart > 1;

        public bool HasNextWindow => WindowEnd < TotalPages;

        public int PreviousWindowPage => Math.Max(WindowStart - PagesPerWindow, 1);

        public int NextWindowPage => WindowEnd + 1;

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

        public int PreviousPage => CurrentPage - 1;

        public int NextPage => CurrentPage + 1;
    }
}
