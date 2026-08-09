using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Infrastructure.Data.Common.ErrorConstants;

namespace LearnLink.Core.Models
{
    public class GradeFormViewModel
    {
        public int Id { get; set; }

        public string StudentFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(2.00, 6.00, ErrorMessage = RangeErrorMessage)]
        public decimal Grade { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a student.")]
        public int? SelectedStudentId { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a subject.")]
        public int? SelectedSubjectId { get; set; }

        public List<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> StudentOptions { get; set; } = new List<SelectListItem>();
    }
}
