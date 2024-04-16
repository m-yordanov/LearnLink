using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Core.Models
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

        public DateTime DateBefore { get; set; }
        
        public DateTime DateAfter { get; set; }

        public int TotalCount { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalPages { get; set; } 
       
        public int PageNumber { get; set; }
        
        public string SelectedStudent { get; set; } = string.Empty;
        
        public string SelectedTeacher { get; set; } = string.Empty;
        
        public string SelectedSubject { get; set; } = string.Empty;
        
        public IEnumerable<Grade> FilteredGrades { get; set; } = Enumerable.Empty<Grade>();
        
        public IEnumerable<SelectListItem> StudentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        
        public IEnumerable<SelectListItem> TeacherOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        
        public IEnumerable<SelectListItem> SubjectOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
