using LearnLink.Infrastructure.Data.Models.Enums;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Infrastructure.Data.SeedDbContext
{
    internal class SeedData
    {
        private static readonly DateTime SeedDateAndTime = new DateTime(2026, 7, 11, 15, 7, 30);

        public ApplicationUser StudentUser { get; set; } = null!;

        public ApplicationUser TeacherUser { get; set; } = null!;

        public ApplicationUser AdminUser { get; set; } = null!;

        public Student Student { get; set; } = null!;

        public Teacher Teacher { get; set; } = null!;

        public Subject FirstSubject { get; private set; } = null!;

        public Subject SecondSubject { get; private set; } = null!;

        public Subject ThirdSubject { get; private set; } = null!;

        public Attendance FirstAttendance { get; private set; } = null!;
        
        public Attendance SecondAttendance { get; private set; } = null!;
        
        public Grade FirstGrade { get; private set; } = null!;
        
        public Grade SecondGrade { get; private set; } = null!;

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
            StudentUser = new ApplicationUser()
            {
                Id = "dea12856-c098-4129-b3f3-b893d8395082",
                UserName = "student@mail.com",
                NormalizedUserName = "student@mail.com",
                Email = "student@mail.com",
                NormalizedEmail = "student@mail.com",
                FirstName = "Ivan",
                LastName = "Petrov",
                ConcurrencyStamp = "f2b24258-c192-4ef8-bfc1-b6c15d2ccf32",
                SecurityStamp = "25e4616e-4818-4d50-b4d4-000bbf56b53e",
                PasswordHash = "AQAAAAEAACcQAAAAEPuDCnn2OViyOCZCJDPhBC7UM/7unLPkmHsM3stDLuG8Z+O47DRS/tp7YAYBP76D/w=="
            };

            TeacherUser = new ApplicationUser()
            {
                Id = "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                UserName = "teacher@mail.com",
                NormalizedUserName = "teacher@mail.com",
                Email = "teacher@mail.com",
                NormalizedEmail = "teacher@mail.com",
                FirstName = "Viktor",
                LastName = "Georgiev",
                ConcurrencyStamp = "e1f83c46-c1a5-4870-8447-b0b399035ac4",
                SecurityStamp = "0db2227e-9f41-46a1-9df9-34e4290b622a",
                PasswordHash = "AQAAAAEAACcQAAAAEHND0K+Y+rnCaFUzR+ussBps/28F7VBGNRvCXbOzv7mfCvU6622kNiFEdGGe1QPbTg=="
            };

            AdminUser = new ApplicationUser()
            {
                Id = "c2b15954-6a87-4207-8f3d-fb93ef5481f4",
                UserName = "admin@mail.com",
                NormalizedUserName = "admin@mail.com",
                Email = "admin@mail.com",
                NormalizedEmail = "admin@mail.com",
                FirstName = "The",
                LastName = "Admin",
                ConcurrencyStamp = "007e0918-b993-45de-a254-8053bc10a141",
                SecurityStamp = "590871c2-bbf8-4e02-9f4e-7b81b5b0e139",
                PasswordHash = "AQAAAAEAACcQAAAAEPlrrhtGUUqffS0i23TiGtrM75PsCR59OF+/L/DrKCk4ari7AwheSuXtHYXAtyc14w=="
            };
        }

        private void SeedStudent()
        {
            Student = new Student()
            {
                Id = 1,
                FirstName = StudentUser.FirstName,
                LastName = StudentUser.LastName,
                Email = StudentUser.Email ?? string.Empty,
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
                Email = TeacherUser.Email ?? string.Empty,
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
                DateAndTime = SeedDateAndTime
            };
            SecondAttendance = new Attendance()
            {
                Id = 2,
                StudentId = Student.Id,
                TeacherId = Student.Id,
                SubjectId = SecondSubject.Id,
                Status = AttendanceStatus.Late,
                DateAndTime = SeedDateAndTime
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
                DateAndTime = SeedDateAndTime
            };
            SecondGrade = new Grade()
            {
                Id = 2,
                StudentId = Student.Id,
                TeacherId = Teacher.Id,
                SubjectId = SecondSubject.Id,
                Value = 5.00M,
                DateAndTime = SeedDateAndTime
            };
        }
    }
}
