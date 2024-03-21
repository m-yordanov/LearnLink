namespace LearnLink.Models
{
    public class GradeViewModel
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;
        
        public decimal Value { get; set; }

        public DateTime DateAndTime { get; set; }

        public string TeacherFirstName { get; set; } = string.Empty;

        public string TeacherLastName { get; set; } = string.Empty;
    }
}
