using Microsoft.AspNetCore.Identity;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;

namespace LearnLink.Data.SeedDbContext
{
    internal class SeedData
    {
        public IdentityUser StudentUser { get; set; }

        public IdentityUser TeacherUser { get; set; }

        public Student Student { get; set; }

        public Teacher Teacher { get; set; }

        public Subject FirstSubject { get; private set; }

        public Subject SecondSubject { get; private set; }

        public Attendance FirstAttendance { get; private set; }
        
        public Attendance SecondAttendance { get; private set; }
        
        public Grade FirstGrade { get; private set; }
        
        public Grade SecondGrade { get; private set; }

        public SeedData()
        {
            SeedUsers();
            SeedStudent();
            SeedTeacher();
            SeedSubjects();
            SeedAttendances();
            SeedGrades();
        }

        private void SeedUsers()
        {
            var hasher = new PasswordHasher<IdentityUser>();

            StudentUser = new IdentityUser()
            {
                Id = "dea12856-c098-4129-b3f3-b893d8395082",
                UserName = "student@mail.com",
                NormalizedUserName = "student@mail.com",
                Email = "student@mail.com",
                NormalizedEmail = "student@mail.com"
            };

            StudentUser.PasswordHash =
                 hasher.HashPassword(StudentUser, "/g_(q(G380B5");

            TeacherUser = new IdentityUser()
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                UserName = "teacher@mail.com",
                NormalizedUserName = "teacher@mail.com",
                Email = "teacher@mail.com",
                NormalizedEmail = "teacher@mail.com"
            };

            TeacherUser.PasswordHash =
            hasher.HashPassword(TeacherUser, "ce7`oR>)S9Y5");
        }

        private void SeedStudent()
        {
            Student = new Student()
            {
                Id = 1,
                FirstName = "Ivan",
                LastName = "Petrov",
                Email = StudentUser.Email,
                UserId = StudentUser.Id
            };
        }

        private void SeedTeacher()
        {
            Teacher = new Teacher()
            {
                Id = 1,
                FirstName = "Viktor",
                LastName = "Georgiev",
                Email = TeacherUser.Email,
                UserId = TeacherUser.Id
            };
        }

        private void SeedSubjects()
        {
            FirstSubject = new Subject()
            {
                Id = 1,
                Name = "History"
            };

            SecondSubject = new Subject()
            {
                Id = 2,
                Name = "Geography"
            };
        }

        private void SeedAttendances()
        {
            FirstAttendance = new Attendance()
            {
                Id = 1,
                StudentId = StudentUser.Id,
                TeacherId = TeacherUser.Id,
                SubjectId = FirstSubject.Id,
                Status = AttendanceStatus.Present,
                DateAndTime = DateTime.Now
            };
            SecondAttendance = new Attendance()
            {
                Id = 2,
                StudentId = StudentUser.Id,
                TeacherId = TeacherUser.Id,
                SubjectId = SecondSubject.Id,
                Status = AttendanceStatus.Late,
                DateAndTime = DateTime.Now
            };
        }

        private void SeedGrades() 
        {
            FirstGrade = new Grade()
            {
                Id = 1,
                StudentId = StudentUser.Id,
                TeacherId = TeacherUser.Id,
                SubjectId = FirstSubject.Id,
                Value = 5.50M,
                DateAndTime = DateTime.Now
            };
            SecondGrade = new Grade()
            {
                Id = 2,
                StudentId = StudentUser.Id,
                TeacherId = TeacherUser.Id,
                SubjectId = SecondSubject.Id,
                Value = 5.00M,
                DateAndTime = DateTime.Now
            };
        }
    }
}
