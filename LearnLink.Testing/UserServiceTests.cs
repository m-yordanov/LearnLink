using LearnLink.Core.Models;
using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using static LearnLink.Core.Constants.RoleConstants;

namespace LearnLink.Testing
{
    [TestFixture]
    public class UserServiceTests
    {
        private LearnLinkDbContext data = null!;
        private Mock<UserManager<ApplicationUser>> userManager = null!;
        private UserService service = null!;

        [SetUp]
        public void SetUp()
        {
            data = TestDb.CreateContext();
            userManager = TestDb.CreateUserManager();
            service = new UserService(data, userManager.Object);
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        [Test]
        public async Task GetFilteredUsersAsync_ReturnsRoleNamesForEachUser()
        {
            var user = data.AddUser("teacher@mail.com", "Viktor", "Georgiev");
            data.AddRole(user, TeacherRole);

            var users = await service.GetFilteredUsersAsync(string.Empty, 1, 10);

            Assert.That(users, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(users[0].FullName, Is.EqualTo("Viktor Georgiev"));
                Assert.That(users[0].Roles, Is.EqualTo(new[] { TeacherRole }));
            });
        }

        [Test]
        public async Task GetFilteredUsersAsync_LabelsUsersWithoutRolesAsNone()
        {
            data.AddUser("nobody@mail.com");

            var users = await service.GetFilteredUsersAsync(string.Empty, 1, 10);

            Assert.That(users[0].Roles, Is.EqualTo(new[] { NoRole }));
        }

        [Test]
        public async Task GetFilteredUsersAsync_MatchesOnEmailAndOnFullName()
        {
            data.AddUser("ivan@mail.com", "Ivan", "Petrov");
            data.AddUser("maria@mail.com", "Maria", "Ivanova");

            var byEmail = await service.GetFilteredUsersAsync("maria@", 1, 10);
            var byFullName = await service.GetFilteredUsersAsync("Ivan Petrov", 1, 10);

            Assert.Multiple(() =>
            {
                Assert.That(byEmail.Select(u => u.Email), Is.EqualTo(new[] { "maria@mail.com" }));
                Assert.That(byFullName.Select(u => u.Email), Is.EqualTo(new[] { "ivan@mail.com" }));
            });
        }

        [Test]
        public async Task GetFilteredUsersAsync_ReturnsRequestedPage()
        {
            data.AddUser("a@mail.com");
            data.AddUser("b@mail.com");
            data.AddUser("c@mail.com");

            var firstPage = await service.GetFilteredUsersAsync(string.Empty, 1, 2);
            var secondPage = await service.GetFilteredUsersAsync(string.Empty, 2, 2);

            Assert.Multiple(() =>
            {
                Assert.That(firstPage.Select(u => u.Email), Is.EqualTo(new[] { "a@mail.com", "b@mail.com" }));
                Assert.That(secondPage.Select(u => u.Email), Is.EqualTo(new[] { "c@mail.com" }));
            });
        }

        [Test]
        public async Task GetFilteredUsersAsync_ReportsLockedOutUsersAsInactive()
        {
            data.AddUser("active@mail.com");
            data.AddUser("locked@mail.com", lockoutEnd: DateTimeOffset.MaxValue);

            var users = await service.GetFilteredUsersAsync(string.Empty, 1, 10);

            Assert.Multiple(() =>
            {
                Assert.That(users.Single(u => u.Email == "active@mail.com").IsActive, Is.True);
                Assert.That(users.Single(u => u.Email == "locked@mail.com").IsActive, Is.False);
            });
        }

        [Test]
        public async Task GetFilteredUsersAsync_StillReportsAUserServingAFailedSignInLockoutAsActive()
        {
            data.AddUser("clumsy@mail.com", lockoutEnd: DateTimeOffset.UtcNow.AddMinutes(15));

            var users = await service.GetFilteredUsersAsync(string.Empty, 1, 10);

            Assert.That(users.Single().IsActive, Is.True,
                "mistyping a password must not make the account look deactivated");
        }

        [Test]
        public async Task GetTotalUsersCountAsync_CountsOnlyMatchingUsers()
        {
            data.AddUser("ivan@mail.com", "Ivan", "Petrov");
            data.AddUser("maria@mail.com", "Maria", "Ivanova");

            Assert.Multiple(async () =>
            {
                Assert.That(await service.GetTotalUsersCountAsync(string.Empty), Is.EqualTo(2));
                Assert.That(await service.GetTotalUsersCountAsync("Petrov"), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task CreateUserAsync_RejectsARoleThatDoesNotExist()
        {
            var viewModel = new UserFormViewModel
            {
                Email = "new@mail.com",
                FirstName = "New",
                LastName = "User",
                Password = "password",
                Role = "Janitor"
            };

            var result = await service.CreateUserAsync(viewModel);

            Assert.That(result.Succeeded, Is.False);
            userManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateUserAsync_CreatesAStudentRecordForTheStudentRole()
        {
            data.Roles.Add(new IdentityRole(StudentRole));
            data.SaveChanges();

            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), StudentRole))
                .ReturnsAsync(IdentityResult.Success);

            var result = await service.CreateUserAsync(new UserFormViewModel
            {
                Email = "student@mail.com",
                FirstName = "Ivan",
                LastName = "Petrov",
                Password = "password",
                Role = StudentRole
            });

            Assert.That(result.Succeeded, Is.True);
            Assert.That(data.Students.Single().Email, Is.EqualTo("student@mail.com"));
        }

        [Test]
        public async Task CreateUserAsync_RemovesTheUserWhenTheRoleCannotBeAssigned()
        {
            data.Roles.Add(new IdentityRole(StudentRole));
            data.SaveChanges();

            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), StudentRole))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "nope" }));

            var result = await service.CreateUserAsync(new UserFormViewModel
            {
                Email = "student@mail.com",
                FirstName = "Ivan",
                LastName = "Petrov",
                Password = "password",
                Role = StudentRole
            });

            Assert.That(result.Succeeded, Is.False);
            Assert.That(data.Students, Is.Empty);
            userManager.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ReturnsUserNotFoundForAnUnknownId()
        {
            userManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var result = await service.DeleteUserAsync("missing");

            Assert.That(result, Is.EqualTo(UserDeleteResult.UserNotFound));
        }

        [Test]
        public async Task DeleteUserAsync_RefusesToDeleteTheLastAdmin()
        {
            var admin = data.AddUser("admin@mail.com");

            userManager.Setup(m => m.FindByIdAsync(admin.Id)).ReturnsAsync(admin);
            userManager.Setup(m => m.IsInRoleAsync(admin, AdminRole)).ReturnsAsync(true);
            userManager.Setup(m => m.GetUsersInRoleAsync(AdminRole)).ReturnsAsync(new List<ApplicationUser> { admin });

            var result = await service.DeleteUserAsync(admin.Id);

            Assert.That(result, Is.EqualTo(UserDeleteResult.LastAdmin));
            userManager.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Test]
        public async Task DeleteUserAsync_RemovesTheStudentRecordAndItsGradesAndAttendances()
        {
            var user = data.AddUser("student@mail.com");
            var student = data.AddStudent(user);
            var teacher = data.AddTeacher(data.AddUser("teacher@mail.com"));
            var subject = data.AddSubject("History");

            data.Grades.Add(new Grade
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                Value = 5.5M,
                DateAndTime = DateTime.Now
            });
            data.Attendances.Add(new Attendance
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                DateAndTime = DateTime.Now
            });
            data.SaveChanges();

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.IsInRoleAsync(user, AdminRole)).ReturnsAsync(false);
            userManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await service.DeleteUserAsync(user.Id);

            Assert.That(result, Is.EqualTo(UserDeleteResult.Success));
            Assert.Multiple(() =>
            {
                Assert.That(data.Grades, Is.Empty);
                Assert.That(data.Attendances, Is.Empty);
                Assert.That(data.Students, Is.Empty);
            });
            userManager.Verify(m => m.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task SetUserActiveAsync_LocksTheAccountAndDeactivatesTheStudentRecord()
        {
            var user = data.AddUser("student@mail.com");
            data.AddStudent(user);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.SetLockoutEnabledAsync(user, true)).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue))
                .ReturnsAsync(IdentityResult.Success);

            var success = await service.SetUserActiveAsync(user.Id, false);

            Assert.That(success, Is.True);
            Assert.That(data.Students.Single().IsActive, Is.False);
            userManager.Verify(m => m.SetLockoutEnabledAsync(user, true), Times.Once);
            userManager.Verify(m => m.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue), Times.Once);
            userManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once);
        }

        [Test]
        public async Task SetUserActiveAsync_StampsTheUserSoTheirExistingSessionStops()
        {
            var user = data.AddUser("teacher@mail.com");
            data.AddTeacher(user);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.SetLockoutEnabledAsync(user, true)).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>()))
                .ReturnsAsync(IdentityResult.Success);

            await service.SetUserActiveAsync(user.Id, false);

            userManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once,
                "the lockout alone leaves the cookie the user already holds working");
        }

        [Test]
        public async Task SetUserActiveAsync_ClearsTheLockoutAndReactivatesTheStudentRecord()
        {
            var user = data.AddUser("student@mail.com", lockoutEnd: DateTimeOffset.MaxValue);
            data.AddStudent(user, isActive: false);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetLockoutEnabledAsync(user)).ReturnsAsync(true);
            userManager.Setup(m => m.SetLockoutEndDateAsync(user, null)).ReturnsAsync(IdentityResult.Success);

            var success = await service.SetUserActiveAsync(user.Id, true);

            Assert.That(success, Is.True);
            Assert.That(data.Students.Single().IsActive, Is.True);
            userManager.Verify(m => m.SetLockoutEndDateAsync(user, null), Times.Once);
        }

        [Test]
        public async Task UnassignRoleAsync_DeactivatesTheTeacherRecordInsteadOfDeletingIt()
        {
            var user = data.AddUser("teacher@mail.com");
            data.AddTeacher(user);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });
            userManager.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            var success = await service.UnassignRoleAsync(user.Id);

            Assert.That(success, Is.True);
            Assert.That(data.Teachers.Single().IsActive, Is.False);
            userManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once,
                "the removed role stays in the cookie until the stamp changes");
        }

        [Test]
        public async Task ChangeUserRoleAsync_RejectsARoleThatDoesNotExist()
        {
            var user = data.AddUser("teacher@mail.com");
            data.AddTeacher(user);
            data.EnsureRole(TeacherRole);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });

            var success = await service.ChangeUserRoleAsync(user.Id, "Janitor");

            Assert.That(success, Is.False);
            Assert.That(data.Teachers.Single().IsActive, Is.True, "a rejected change must not touch the record");
            userManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ChangeUserRoleAsync_KeepsTheRecordActiveWhenTheRoleIsUnchanged()
        {
            var user = data.AddUser("teacher@mail.com");
            data.AddTeacher(user);
            data.EnsureRole(TeacherRole);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });

            var success = await service.ChangeUserRoleAsync(user.Id, TeacherRole);

            Assert.That(success, Is.True);
            Assert.That(data.Teachers.Single().IsActive, Is.True,
                "re-assigning the role a user already has must not deactivate their record");
            userManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            userManager.Verify(m => m.UpdateSecurityStampAsync(It.IsAny<ApplicationUser>()), Times.Never,
                "a no-op change must not sign the user out");
        }

        [Test]
        public async Task ChangeUserRoleAsync_StampsTheUserWhenTheRoleChanges()
        {
            var user = data.AddUser("person@mail.com");
            data.EnsureRole(StudentRole);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });
            userManager.Setup(m => m.AddToRoleAsync(user, StudentRole)).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            await service.ChangeUserRoleAsync(user.Id, StudentRole);

            userManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once,
                "a demoted user keeps their old rights until the cookie is rejected");
        }

        [Test]
        public async Task ChangeUserRoleAsync_StampsTheUserWhenTheyHadNoRoleBefore()
        {
            var user = data.AddUser("person@mail.com");
            data.EnsureRole(StudentRole);

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            userManager.Setup(m => m.AddToRoleAsync(user, StudentRole)).ReturnsAsync(IdentityResult.Success);

            await service.ChangeUserRoleAsync(user.Id, StudentRole);

            userManager.Verify(m => m.UpdateSecurityStampAsync(user), Times.Once);
        }

        [Test]
        public async Task ChangeUserRoleAsync_AssignsTheNewRoleBeforeRemovingTheOldOne()
        {
            var user = data.AddUser("person@mail.com");
            data.EnsureRole(StudentRole);

            var sequence = new List<string>();

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });
            userManager.Setup(m => m.AddToRoleAsync(user, StudentRole))
                .ReturnsAsync(IdentityResult.Success)
                .Callback(() => sequence.Add("add"));
            userManager.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback(() => sequence.Add("remove"));

            await service.ChangeUserRoleAsync(user.Id, StudentRole);

            Assert.That(sequence, Is.EqualTo(new[] { "add", "remove" }));
        }

        [Test]
        public async Task ChangeUserRoleAsync_KeepsGradesWhenATeacherBecomesAStudent()
        {
            var user = data.AddUser("person@mail.com");
            data.EnsureRole(StudentRole);
            var teacher = data.AddTeacher(user);
            var otherStudent = data.AddStudent(data.AddUser("pupil@mail.com"));
            var subject = data.AddSubject("History");

            data.Grades.Add(new Grade
            {
                StudentId = otherStudent.Id,
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                Value = 5.5M,
                DateAndTime = DateTime.Now
            });
            data.SaveChanges();

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { TeacherRole });
            userManager.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(m => m.AddToRoleAsync(user, StudentRole)).ReturnsAsync(IdentityResult.Success);

            var success = await service.ChangeUserRoleAsync(user.Id, StudentRole);

            Assert.That(success, Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(data.Grades.Count(), Is.EqualTo(1), "the grade the teacher issued must survive");
                Assert.That(data.Teachers.Single(t => t.UserId == user.Id).IsActive, Is.False);
                Assert.That(data.Students.Any(s => s.UserId == user.Id), Is.True);
            });
        }

        [Test]
        public async Task GetUserForDeleteAsync_CountsTheRecordsThatWouldBeDeleted()
        {
            var user = data.AddUser("student@mail.com", "Ivan", "Petrov");
            var student = data.AddStudent(user);
            var teacher = data.AddTeacher(data.AddUser("teacher@mail.com"));
            var subject = data.AddSubject("History");

            data.Grades.AddRange(
                new Grade { StudentId = student.Id, TeacherId = teacher.Id, SubjectId = subject.Id, Value = 5.5M, DateAndTime = DateTime.Now },
                new Grade { StudentId = student.Id, TeacherId = teacher.Id, SubjectId = subject.Id, Value = 4.0M, DateAndTime = DateTime.Now });
            data.Attendances.Add(new Attendance { StudentId = student.Id, TeacherId = teacher.Id, SubjectId = subject.Id, DateAndTime = DateTime.Now });
            data.SaveChanges();

            userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { StudentRole });

            var viewModel = await service.GetUserForDeleteAsync(user.Id);

            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel!.FullName, Is.EqualTo("Ivan Petrov"));
                Assert.That(viewModel.GradesCount, Is.EqualTo(2));
                Assert.That(viewModel.AttendancesCount, Is.EqualTo(1));
            });
        }
    }
}
