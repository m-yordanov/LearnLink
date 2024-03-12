using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LearnLink.Data.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
