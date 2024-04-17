using LearnLink.Core.Interfaces;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Core.Services
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

            string? oldRole = existingRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(oldRole))
            {
                await userManager.RemoveFromRolesAsync(user, existingRoles);
            }

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return false;
            }

            if (roleName == "Teacher")
            {
                await MapUserToTeacherAsync(user);
            }
            else if (roleName == "Student")
            {
                await MapUserToStudentAsync(user);
            }

            if (oldRole == "Teacher")
            {
                await RemoveUserFromTeacherAsync(user);
            }
            else if (oldRole == "Student")
            {
                await RemoveUserFromStudentAsync(user);
            }

            return true;
        }


        public async Task<bool> UnassignRoleAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var existingRoles = await userManager.GetRolesAsync(user);
            if (existingRoles != null && existingRoles.Count > 0)
            {
                var result = await userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!result.Succeeded)
                {
                    return false;
                }

                await userManager.RemoveFromRolesAsync(user, existingRoles);

                return true;
            }

            return false;
        }

        private async Task MapUserToTeacherAsync(ApplicationUser user)
        {
            var existingTeacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (existingTeacher == null)
            {
                var newTeacher = new Teacher
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                data.Teachers.Add(newTeacher);
                await data.SaveChangesAsync();
            }
        }

        private async Task MapUserToStudentAsync(ApplicationUser user)
        {
            var existingStudent = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudent == null)
            {
                var newStudent = new Student
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                data.Students.Add(newStudent);
                await data.SaveChangesAsync();
            }
        }

        private async Task RemoveUserFromTeacherAsync(ApplicationUser user)
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher != null)
            {
                data.Teachers.Remove(teacher);
                await data.SaveChangesAsync();
            }
        }

        private async Task RemoveUserFromStudentAsync(ApplicationUser user)
        {
            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                data.Students.Remove(student);
                await data.SaveChangesAsync();
            }
        }
    }
}
