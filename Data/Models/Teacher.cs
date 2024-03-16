using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Data.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(MaxNameLength)]
        string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(MaxNameLength)]
        string LastName { get; set; } = string.Empty;

        [Required]
        string Email { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
