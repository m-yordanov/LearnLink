using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Models
{
    public class GradeViewModel
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;
        
        public decimal Value { get; set; }

        public DateTime DateAndTime { get; set; }

        public string StudentFirstName { get; set; } = string.Empty;

        public string StudentLastName { get; set; } = string.Empty;

        public string TeacherFirstName { get; set; } = string.Empty;

        public string TeacherLastName { get; set; } = string.Empty;

        public string SelectedStudent { get; set; } = string.Empty;

        public string SelectedTeacher { get; set; } = string.Empty;
        
        public string SelectedSubject { get; set; } = string.Empty;
        
        public DateTime SelectedDate { get; set; }

        public IEnumerable<GradeViewModel> Grades { get; set; } = null!;

        public IEnumerable<SelectListItem> StudentOptions { get; set; } = null!;

        public IEnumerable<SelectListItem> TeacherOptions { get; set; } = null!;

        public IEnumerable<SelectListItem> SubjectOptions { get; set; } = null!;
    }
}
