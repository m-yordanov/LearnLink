using System.ComponentModel.DataAnnotations;

namespace LearnLink.Data.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
