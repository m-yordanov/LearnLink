using LearnLink.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace LearnLink.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetFilteredUsersAsync(string searchString, int page, int pageSize);

        Task<int> GetTotalUsersCountAsync(string searchString);

        Task<List<string>> GetAllRolesAsync();

        Task<IdentityResult> CreateUserAsync(UserFormViewModel viewModel);

        Task<bool> ChangeUserRoleAsync(string userId, string roleName);

        Task<bool> UnassignRoleAsync(string userId);

        Task<bool> SetUserActiveAsync(string userId, bool isActive);

        Task<UserDeleteViewModel?> GetUserForDeleteAsync(string userId);

        Task<UserDeleteResult> DeleteUserAsync(string userId);
    }
}
