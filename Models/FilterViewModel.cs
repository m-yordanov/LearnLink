namespace LearnLink.Models
{
    public class FilterViewModel
    {
        public string SelectedStudent { get; set; } = string.Empty;

        public string SelectedTeacher { get; set; } = string.Empty;
        
        public string SelectedSubject { get; set; } = string.Empty;
        
        public DateTime SelectedDate { get; set; }
    }
}
