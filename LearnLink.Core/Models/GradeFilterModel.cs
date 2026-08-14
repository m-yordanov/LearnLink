using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Core.Models
{
    public class GradeFilterModel
    {
        public string SelectedStudent { get; set; } = string.Empty;

        public string SelectedTeacher { get; set; } = string.Empty;

        public string SelectedSubject { get; set; } = string.Empty;

        public DateTime? DateBefore { get; set; }

        public DateTime? DateAfter { get; set; }

        public string SortBy { get; set; } = "date";

        public bool SortDescending { get; set; } = true;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = maxPerPage;
    }
}
