using LearnLink.Models;

namespace LearnLink.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllUsersWithRolesAsync();

        Task<List<string>> GetAllRolesAsync();

        Task<bool> ChangeUserRoleAsync(string userId, string roleName);
    }
}
