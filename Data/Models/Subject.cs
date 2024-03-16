using System.ComponentModel.DataAnnotations;
using static LearnLink.Data.Constants.DataConstants;

namespace LearnLink.Data.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = string.Empty;
    }
}
