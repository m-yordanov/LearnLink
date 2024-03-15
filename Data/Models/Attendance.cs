using System.ComponentModel.DataAnnotations;
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
        public int SubjectId { get; set;}

        [Required]
        public AttendanceStatus Status { get; set; }


        [Required]
        public DateTime DateAndTime { get; set; }
    }
}
