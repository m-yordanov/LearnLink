using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Data.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
