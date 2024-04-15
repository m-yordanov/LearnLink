﻿using LearnLink.Infrastructure.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Infrastructure.Data.Common.ErrorConstants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearnLink.Core.Models
{
    public class AttendanceFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        public int SelectedStudentId { get; set; }

        [Required(ErrorMessage = "Please select a subject.")]
        public int SelectedSubjectId { get; set; }

        [Required(ErrorMessage = "Please select the attendance status.")]
        public AttendanceStatus Status { get; set; }

        public DateTime DateAndTime { get; set; }

        public List<SelectListItem> StudentOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> SubjectOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

        public bool AttendanceAddedSuccessfully { get; set; }
    }
}
