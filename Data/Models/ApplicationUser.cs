using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; } = string.Empty;
    }
}
