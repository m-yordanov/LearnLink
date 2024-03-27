using LearnLink.Data.Models;

namespace LearnLink.Models
{
    public class StudentHomeViewModel
    {
        public string FirstName { get; set; } = string.Empty;

        public decimal Grade { get; set; }

        public IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
