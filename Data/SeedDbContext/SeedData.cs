using Microsoft.AspNetCore.Identity;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;

namespace LearnLink.Data.SeedDbContext
{
    internal class SeedData
    {
        public ApplicationUser StudentUser { get; set; }

        public ApplicationUser TeacherUser { get; set; }

        public ApplicationUser AdminUser { get; set; }

        public Student Student { get; set; }

        public Teacher Teacher { get; set; }

        public Subject FirstSubject { get; private set; }

        public Subject SecondSubject { get; private set; }

        public Subject ThirdSubject { get; private set; }

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
            var hasher = new PasswordHasher<ApplicationUser>();

            StudentUser = new ApplicationUser()
            {
                Id = "dea12856-c098-4129-b3f3-b893d8395082",
                UserName = "student@mail.com",
                NormalizedUserName = "student@mail.com",
                Email = "student@mail.com",
                NormalizedEmail = "student@mail.com",
                FirstName = "Ivan",
                LastName = "Petrov"
            };

            StudentUser.PasswordHash =
                 hasher.HashPassword(StudentUser, "/g_(q(G380B5");

            TeacherUser = new ApplicationUser()
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                UserName = "teacher@mail.com",
                NormalizedUserName = "teacher@mail.com",
                Email = "teacher@mail.com",
                NormalizedEmail = "teacher@mail.com",
                FirstName = "Viktor",
                LastName = "Georgiev"
            };

            TeacherUser.PasswordHash =
            hasher.HashPassword(TeacherUser, "ce7`oR>)S9Y5");

            AdminUser = new ApplicationUser()
            {
                Id = "c2b15954-6a87-4207-8f3d-fb93ef5481f4",
                UserName = "admin@mail.com",
                NormalizedUserName = "admin@mail.com",
                Email = "admin@mail.com",
                NormalizedEmail = "admin@mail.com",
                FirstName = "The",
                LastName = "Admin"
            };

            AdminUser.PasswordHash =
            hasher.HashPassword(AdminUser, "3Z4ZSLc1jTXxYiD");
        }

        private void SeedStudent()
        {
            Student = new Student()
            {
                Id = 1,
                FirstName = StudentUser.FirstName,
                LastName = StudentUser.LastName,
                Email = StudentUser.Email,
                UserId = StudentUser.Id
            };
        }

        private void SeedTeacher()
        {
            Teacher = new Teacher()
            {
                Id = 1,
                FirstName = TeacherUser.FirstName,
                LastName = TeacherUser.LastName,
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
            ThirdSubject = new Subject()
            {
                Id = 3,
                Name = "Mathematics"
            };
        }

        private void SeedAttendances()
        {
            FirstAttendance = new Attendance()
            {
                Id = 1,
                StudentId = Student.Id,
                TeacherId = Teacher.Id,
                SubjectId = FirstSubject.Id,
                Status = AttendanceStatus.Present,
                DateAndTime = DateTime.Now
            };
            SecondAttendance = new Attendance()
            {
                Id = 2,
                StudentId = Student.Id,
                TeacherId = Student.Id,
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
                StudentId = Student.Id,
                TeacherId = Teacher.Id,
                SubjectId = FirstSubject.Id,
                Value = 5.50M,
                DateAndTime = DateTime.Now
            };
            SecondGrade = new Grade()
            {
                Id = 2,
                StudentId = Student.Id,
                TeacherId = Teacher.Id,
                SubjectId = SecondSubject.Id,
                Value = 5.00M,
                DateAndTime = DateTime.Now
            };
        }
    }
}
