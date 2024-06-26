﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnLink.Infrastructure.Data.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey (nameof(TeacherId))]
        public Teacher Teacher { get; set;} = null!;

        [Required]
        [Range(2.00, 6.00, ErrorMessage = "Grade must be between 2.00 and 6.00.")]
        public decimal Value { get; set; }

        [Required]
        public DateTime DateAndTime { get; set; }
    }
}
