using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Data.Models
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
