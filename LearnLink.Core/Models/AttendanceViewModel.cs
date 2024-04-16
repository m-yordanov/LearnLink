using LearnLink.Infrastructure.Data.Models;
using LearnLink.Infrastructure.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Core.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }

        public DateTime DateAndTime { get; set; }

        public AttendanceStatus Status { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string StudentFirstName { get; set; } = string.Empty;

        public string StudentLastName { get; set; } = string.Empty;

        public string TeacherFirstName { get; set; } = string.Empty;

        public string TeacherLastName { get; set; } = string.Empty;

        public string SelectedStudent { get; set; } = string.Empty;

        public string SelectedTeacher { get; set; } = string.Empty;

        public string SelectedSubject { get; set; } = string.Empty;

        public string SelectedStatus { get; set; } = string.Empty;

        public DateTime DateBefore { get; set; }

        public DateTime DateAfter { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<Attendance> FilteredAttendances { get; set; } = new List<Attendance>();

        public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();
    }
}
