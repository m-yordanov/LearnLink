namespace LearnLink.Core.Models
{
    public class SubjectViewModel
    {
        public int Id { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public List<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();

        public string SearchString { get; set; } = string.Empty;
        
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalCount { get; set; }
        
        public int TotalPages { get; set; } 

    }
}
