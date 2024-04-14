using System.ComponentModel.DataAnnotations;
using static LearnLink.Infrastructure.Data.Common.DataConstants;

namespace LearnLink.Infrastructure.Data.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SubjectMaxNameLength)]
        public string Name { get; set; } = string.Empty;
    }
}
