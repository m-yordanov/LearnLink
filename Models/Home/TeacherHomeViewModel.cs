using LearnLink.Data.Models.Enums;
using LearnLink.Data.Models;

namespace LearnLink.Models.Home
{
    public class TeacherHomeViewModel
    {

        public string FirstName { get; set; } = string.Empty;

        public decimal GradeValue { get; set; }

        public string Subject { get; set; } = string.Empty;

        public AttendanceStatus Status { get; set; }

        public IEnumerable<Grade> Grades { get; set; } = new List<Grade>();

        public IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
