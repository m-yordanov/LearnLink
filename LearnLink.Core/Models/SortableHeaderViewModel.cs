namespace LearnLink.Core.Models
{
    public class SortableHeaderViewModel
    {
        public string Column { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string CurrentSortBy { get; set; } = string.Empty;

        public bool CurrentSortDescending { get; set; }

        public bool DefaultDescending { get; set; }

        public int PageSize { get; set; }

        public string PageParameter { get; set; } = "pageNumber";

        public Dictionary<string, string?> Filters { get; set; } = new Dictionary<string, string?>();

        public bool IsActive => string.Equals(Column, CurrentSortBy, StringComparison.OrdinalIgnoreCase);

        public bool NextDescending => IsActive ? !CurrentSortDescending : DefaultDescending;

        public string Indicator => IsActive ? (CurrentSortDescending ? "▼" : "▲") : string.Empty;
    }
}
