namespace LearnLink.Core.Models
{
    public class UserDeleteViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();

        public int GradesCount { get; set; }

        public int AttendancesCount { get; set; }
    }
}
