using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LearnLink.Core.Models
{
    public class GradeViewModel
    {
        public GradeViewModel()
        {
            FilteredGrades = new List<Grade>();
            StudentOptions = new List<SelectListItem>();
            TeacherOptions = new List<SelectListItem>();
            SubjectOptions = new List<SelectListItem>();
        }

        public int Id { get; set; }

		public string Subject { get; set; }

        public decimal Value { get; set; }

        public DateTime DateAndTime { get; set; }
        
        public string StudentFirstName { get; set; }
        
        public string StudentLastName { get; set; }
        
        public string TeacherFirstName { get; set; }
        
        public string TeacherLastName { get; set; }

        public DateTime DateBefore { get; set; }
        
        public DateTime DateAfter { get; set; }
        
        public int CurrentPage { get; set; }
        
        public int TotalCount { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalPages { get; set; } 
       
        public int PageNumber { get; set; }
        
        public string SelectedStudent { get; set; }
        
        public string SelectedTeacher { get; set; }
        
        public string SelectedSubject { get; set; }
        
        public IEnumerable<Grade> FilteredGrades { get; set; }
        
        public IEnumerable<SelectListItem> StudentOptions { get; set; }
        
        public IEnumerable<SelectListItem> TeacherOptions { get; set; }
        
        public IEnumerable<SelectListItem> SubjectOptions { get; set; }
    }
}
