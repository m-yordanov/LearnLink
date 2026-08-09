using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace LearnLink.Testing
{
    internal static class TestDb
    {
        public static LearnLinkDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LearnLinkDbContext>()
                .UseInMemoryDatabase($"LearnLink-{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new LearnLinkDbContext(options);
            context.Database.EnsureCreated();

            // doing every test on an empty database
            context.Grades.RemoveRange(context.Grades);
            context.Attendances.RemoveRange(context.Attendances);
            context.Students.RemoveRange(context.Students);
            context.Teachers.RemoveRange(context.Teachers);
            context.Subjects.RemoveRange(context.Subjects);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            return context;
        }

        public static Mock<UserManager<ApplicationUser>> CreateUserManager()
        {
            return new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);
        }

        public static ApplicationUser AddUser(
            this LearnLinkDbContext data,
            string email,
            string firstName = "Test",
            string lastName = "User",
            DateTimeOffset? lockoutEnd = null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                LockoutEnd = lockoutEnd
            };

            data.Users.Add(user);
            data.SaveChanges();

            return user;
        }

        public static IdentityRole EnsureRole(this LearnLinkDbContext data, string roleName)
        {
            var role = data.Roles.FirstOrDefault(r => r.Name == roleName);

            if (role == null)
            {
                role = new IdentityRole(roleName) { NormalizedName = roleName.ToUpperInvariant() };
                data.Roles.Add(role);
                data.SaveChanges();
            }

            return role;
        }

        public static void AddRole(this LearnLinkDbContext data, ApplicationUser user, string roleName)
        {
            var role = data.EnsureRole(roleName);

            data.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
            data.SaveChanges();
        }

        public static Student AddStudent(this LearnLinkDbContext data, ApplicationUser user, bool isActive = true)
        {
            var student = new Student
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = isActive
            };

            data.Students.Add(student);
            data.SaveChanges();

            return student;
        }

        public static Teacher AddTeacher(this LearnLinkDbContext data, ApplicationUser user, bool isActive = true)
        {
            var teacher = new Teacher
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = isActive
            };

            data.Teachers.Add(teacher);
            data.SaveChanges();

            return teacher;
        }

        public static Subject AddSubject(this LearnLinkDbContext data, string name)
        {
            var subject = new Subject { Name = name };

            data.Subjects.Add(subject);
            data.SaveChanges();

            return subject;
        }
    }
}
