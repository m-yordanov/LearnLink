using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Services
{
    public class UserService : IUserService
    {
        private readonly LearnLinkDbContext data;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(LearnLinkDbContext context, UserManager<ApplicationUser> _userManager)
        {
            data = context;
            userManager = _userManager;
        }

        public async Task<List<UserViewModel>> GetAllUsersWithRolesAsync()
        {
            var usersWithRoles = await data.Users
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Roles = data.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Select(ur => ur.RoleId)
                        .ToList()
                })
                .ToListAsync();

            foreach (var user in usersWithRoles)
            {
                var roleNames = new List<string>();
                foreach (var roleId in user.Roles)
                {
                    var role = await data.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                    if (role != null)
                        roleNames.Add(role.Name);
                }
                user.Roles = roleNames.Any() ? roleNames : new List<string> { "None" };
            }

            return usersWithRoles;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await data.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false; 
            }

            var existingRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, existingRoles);

            var result = await userManager.AddToRoleAsync(user, roleName);

            return result.Succeeded;
        }
    }
}
