using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LearnLink.Data.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        string FirstName { get; set; } = string.Empty;

        [Required]
        string LastName { get; set; } = string.Empty;

        [Required]
        string Email { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
