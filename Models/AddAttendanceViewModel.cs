using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Common.ErrorConstants;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Models
{
    public class AddAttendanceViewModel
    {
        [Required(ErrorMessage = "Please select a student.")]
        public int SelectedStudentId { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        public int SelectedSubjectId { get; set; }

        [Required(ErrorMessage = "Please select the attendance status.")]
        public AttendanceStatus Status { get; set; } 

        [Display(Name = "Date and Time")]
        [Required(ErrorMessage = RequiredErrorMessage)]
        [DisplayFormat(DataFormatString = DateTimeFormat, ApplyFormatInEditMode = true)]
        public DateTime DateAndTime { get; set; }

        public List<SelectListItem> StudentOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        public bool AttendanceAddedSuccessfully { get; set; }
    }
}
