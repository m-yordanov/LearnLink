using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static LearnLink.Infrastructure.Data.Common.DataConstants;

namespace LearnLink.Infrastructure.Data.Models
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
