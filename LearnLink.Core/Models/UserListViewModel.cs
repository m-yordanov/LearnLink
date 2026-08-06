namespace LearnLink.Core.Models
{
    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        public List<string> RoleOptions { get; set; } = new List<string>();

        public string SearchString { get; set; } = string.Empty;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
