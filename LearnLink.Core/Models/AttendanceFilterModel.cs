using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Core.Models
{
    public class AttendanceFilterModel
    {
        public string SelectedStudent { get; set; } = string.Empty;

        public string SelectedTeacher { get; set; } = string.Empty;

        public string SelectedSubject { get; set; } = string.Empty;

        public string SelectedStatus { get; set; } = string.Empty;

        public DateTime? DateBefore { get; set; }

        public DateTime? DateAfter { get; set; }

        public string SortBy { get; set; } = "date";

        public bool SortDescending { get; set; } = true;

        private int pageNumber = 1;

        private int pageSize = maxPerPage;

        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = ClampPageNumber(value);
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = ClampPageSize(value);
        }
    }
}
