using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LearnLink.Data.Models.Enums;

namespace LearnLink.Data.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public int SubjectId { get; set;}

        [Required]
        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;

        [Required]
        public AttendanceStatus Status { get; set; }


        [Required]
        public DateTime DateAndTime { get; set; }
    }
}
