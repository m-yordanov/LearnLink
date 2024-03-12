using System.ComponentModel.DataAnnotations;

namespace LearnLink.Data.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
    }
}
