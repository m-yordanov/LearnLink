using System.ComponentModel.DataAnnotations;

namespace LearnLink.Data.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public Student Stundent { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public Subject Subject { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public Teacher Teacher { get; set;} = null!;

        [Required]
        public int Value { get; set; }

        [Required]
        public DateTime DateAndTime { get; set; }
    }
}
