using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Common.ErrorConstants;

namespace LearnLink.Models
{
    public class AddGradeViewModel
    {
        public string StudentFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(2.00, 6.00, ErrorMessage = RangeErrorMessage)]
        public decimal Grade { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int SelectedStudentId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int SelectedSubjectId { get; set; }

        public List<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> StudentOptions { get; set; } = new List<SelectListItem>();

        public bool GradeAddedSuccessfully { get; internal set; }
    }
}
