using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllUsersWithRolesAsync();

        Task<List<string>> GetAllRolesAsync();

        Task<bool> ChangeUserRoleAsync(string userId, string roleName);
    }
}
