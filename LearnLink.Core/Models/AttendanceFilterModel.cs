namespace LearnLink.Core.Models
{
    public class AttendanceFilterModel : ListFilterModel
    {
        public string SelectedStatus { get; set; } = string.Empty;
    }
}
