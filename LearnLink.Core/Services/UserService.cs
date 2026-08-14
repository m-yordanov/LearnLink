using LearnLink.Core.Interfaces;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static LearnLink.Core.Constants.RoleConstants;

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

        public async Task<List<UserViewModel>> GetFilteredUsersAsync(string searchString, int page, int pageSize)
        {
            var now = DateTimeOffset.UtcNow;

            var users = await FilterUsers(searchString)
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = $"{user.FirstName} {user.LastName}",
                    IsActive = user.LockoutEnd == null || user.LockoutEnd < DeactivationThreshold,
                    Roles = data.Roles
                        .Where(r => data.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == r.Id))
                        .Select(r => r.Name ?? string.Empty)
                        .ToList()
                })
                .ToListAsync();

            foreach (var user in users.Where(u => !u.Roles.Any()))
            {
                user.Roles.Add(NoRole);
            }

            return users;
        }

        public async Task<int> GetTotalUsersCountAsync(string searchString)
        {
            return await FilterUsers(searchString).CountAsync();
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await data.Roles.Select(r => r.Name ?? string.Empty).ToListAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(UserFormViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Role)
                && !await data.Roles.AnyAsync(r => r.Name == viewModel.Role))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "The selected role does not exist."
                });
            }

            var user = new ApplicationUser
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName
            };

            var result = await userManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                return result;
            }

            if (string.IsNullOrEmpty(viewModel.Role))
            {
                return IdentityResult.Success;
            }

            var roleResult = await userManager.AddToRoleAsync(user, viewModel.Role);

            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);

                return roleResult;
            }

            if (viewModel.Role == TeacherRole)
            {
                await MapUserToTeacherAsync(user);
            }
            else if (viewModel.Role == StudentRole)
            {
                await MapUserToStudentAsync(user);
            }

            return IdentityResult.Success;
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (!await data.Roles.AnyAsync(r => r.Name == roleName))
            {
                return false;
            }

            var existingRoles = await userManager.GetRolesAsync(user);

            var roleWasAdded = !existingRoles.Contains(roleName);

            if (roleWasAdded)
            {
                var result = await userManager.AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            var replacedRoles = existingRoles.Where(r => r != roleName).ToList();

            if (replacedRoles.Any())
            {
                await userManager.RemoveFromRolesAsync(user, replacedRoles);
            }

            if (roleName == TeacherRole)
            {
                await MapUserToTeacherAsync(user);
            }
            else if (roleName == StudentRole)
            {
                await MapUserToStudentAsync(user);
            }

            if (replacedRoles.Contains(TeacherRole))
            {
                await DeactivateTeacherAsync(user);
            }

            if (replacedRoles.Contains(StudentRole))
            {
                await DeactivateStudentAsync(user);
            }

            if (roleWasAdded || replacedRoles.Any())
            {
                await userManager.UpdateSecurityStampAsync(user);
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

                if (existingRoles.Contains(TeacherRole))
                {
                    await DeactivateTeacherAsync(user);
                }

                if (existingRoles.Contains(StudentRole))
                {
                    await DeactivateStudentAsync(user);
                }

                await userManager.UpdateSecurityStampAsync(user);

                return true;
            }

            return false;
        }

        public async Task<bool> SetUserActiveAsync(string userId, bool isActive)
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

            if (!isActive)
            {
                await userManager.SetLockoutEnabledAsync(user, true);

                var result = await userManager.SetLockoutEndDateAsync(user, DeactivatedLockoutEnd);

                if (!result.Succeeded)
                {
                    return false;
                }
            }
            else if (await userManager.GetLockoutEnabledAsync(user))
            {
                var result = await userManager.SetLockoutEndDateAsync(user, null);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            await userManager.UpdateSecurityStampAsync(user);

            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                student.IsActive = isActive;
            }

            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher != null)
            {
                teacher.IsActive = isActive;
            }

            await data.SaveChangesAsync();

            return true;
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
                Email = user.Email ?? string.Empty,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles.Any() ? roles.ToList() : new List<string> { NoRole }
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

            if (await userManager.IsInRoleAsync(user, AdminRole))
            {
                var admins = await userManager.GetUsersInRoleAsync(AdminRole);

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

        private IQueryable<ApplicationUser> FilterUsers(string searchString)
        {
            var query = data.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u => (u.Email ?? string.Empty).Contains(searchString)
                    || (u.FirstName + " " + u.LastName).Contains(searchString));
            }

            return query;
        }

        private async Task MapUserToTeacherAsync(ApplicationUser user)
        {
            var existingTeacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (existingTeacher == null)
            {
                var newTeacher = new Teacher
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                data.Teachers.Add(newTeacher);
            }
            else
            {
                existingTeacher.IsActive = true;
            }

            await data.SaveChangesAsync();
        }

        private async Task MapUserToStudentAsync(ApplicationUser user)
        {
            var existingStudent = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudent == null)
            {
                var newStudent = new Student
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                data.Students.Add(newStudent);
            }
            else
            {
                existingStudent.IsActive = true;
            }

            await data.SaveChangesAsync();
        }

        private async Task DeactivateTeacherAsync(ApplicationUser user)
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher != null)
            {
                teacher.IsActive = false;
                await data.SaveChangesAsync();
            }
        }

        private async Task DeactivateStudentAsync(ApplicationUser user)
        {
            var student = await data.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student != null)
            {
                student.IsActive = false;
                await data.SaveChangesAsync();
            }
        }
    }
}
