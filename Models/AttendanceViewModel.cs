namespace LearnLink.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }

        public DateTime DateAndTime { get; set; }
        
        public string Status { get; set; } = string.Empty;
        
        public string Subject { get; set; } = string.Empty;

        public string TeacherFirstName { get; set; } = string.Empty;

        public string TeacherLastName { get; set; } = string.Empty;
    }
}
