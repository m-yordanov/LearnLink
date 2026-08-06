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

        public async Task<UserDeleteViewModel?> GetUserForDeleteAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);

            var viewModel = new UserDeleteViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles.Any() ? roles.ToList() : new List<string> { "None" }
            };

            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                viewModel.GradesCount += await data.Grades.CountAsync(g => g.StudentId == student.Id);
                viewModel.AttendancesCount += await data.Attendances.CountAsync(a => a.StudentId == student.Id);
            }

            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher != null)
            {
                viewModel.GradesCount += await data.Grades.CountAsync(g => g.TeacherId == teacher.Id);
                viewModel.AttendancesCount += await data.Attendances.CountAsync(a => a.TeacherId == teacher.Id);
            }

            return viewModel;
        }

        public async Task<UserDeleteResult> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return UserDeleteResult.UserNotFound;
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return UserDeleteResult.UserNotFound;
            }

            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await userManager.GetUsersInRoleAsync("Admin");

                if (admins.Count <= 1)
                {
                    return UserDeleteResult.LastAdmin;
                }
            }

            using var transaction = await data.Database.BeginTransactionAsync();

            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                data.Grades.RemoveRange(await data.Grades.Where(g => g.StudentId == student.Id).ToListAsync());
                data.Attendances.RemoveRange(await data.Attendances.Where(a => a.StudentId == student.Id).ToListAsync());
                data.Students.Remove(student);
            }

            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher != null)
            {
                data.Grades.RemoveRange(await data.Grades.Where(g => g.TeacherId == teacher.Id).ToListAsync());
                data.Attendances.RemoveRange(await data.Attendances.Where(a => a.TeacherId == teacher.Id).ToListAsync());
                data.Teachers.Remove(teacher);
            }

            await data.SaveChangesAsync();

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();

                return UserDeleteResult.Failed;
            }

            await transaction.CommitAsync();

            return UserDeleteResult.Success;
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
