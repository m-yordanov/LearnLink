using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using LearnLink.Services;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Areas.Admin.Controllers
{
    public class ActionsController : AdminBaseController
    {
        private readonly LearnLinkDbContext data;
        private readonly IGradeService gradeService;

        public ActionsController(LearnLinkDbContext context, IGradeService _gradeService)
        {
            data = context;
            gradeService = _gradeService;
        }


        public async Task<IActionResult> AllGrades(string selectedStudent, string selectedTeacher, string selectedSubject, DateTime? dateBefore, DateTime? dateAfter, int pageNumber = 1, int pageSize = 8)
        {
            var gradesViewModel = await gradeService.GetFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter, pageNumber, pageSize);
            var totalFilteredGrades = await gradeService.GetTotalFilteredGradesAsync(selectedStudent, selectedTeacher, selectedSubject, dateBefore, dateAfter);

            int totalPages = (int)Math.Ceiling((double)totalFilteredGrades / pageSize);

            var grades = gradesViewModel.Select(g => new Grade
            {
                Id = g.Id,
                Subject = new Subject { Name = g.Subject },
                Student = new Student { FirstName = g.StudentFirstName, LastName = g.StudentLastName },
                Teacher = new Teacher { FirstName = g.TeacherFirstName, LastName = g.TeacherLastName },
                Value = g.Value,
                DateAndTime = g.DateAndTime
            });

            var viewModel = new GradeViewModel
            {
                FilteredGrades = grades,
                TotalCount = totalFilteredGrades,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                SelectedStudent = selectedStudent,
                SelectedTeacher = selectedTeacher,
                SelectedSubject = selectedSubject
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grade = await data.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            var students = await data.Students.ToListAsync();
            var subjects = await data.Subjects.ToListAsync();

            var viewModel = new GradeFormViewModel
            {
                Id = grade.Id,
                StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}",
                }).ToList(),
                SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(int id, GradeFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                var students = await data.Students.ToListAsync();
                var subjects = await data.Subjects.ToListAsync();

                viewModel.StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList();

                viewModel.SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

                return View(viewModel);
            }


            var grade = await data.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            grade.SubjectId = viewModel.SelectedSubjectId;
            grade.StudentId = viewModel.SelectedStudentId;
            grade.Value = viewModel.Grade;

            data.Update(grade);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(AllGrades));
        }

        public async Task<IActionResult> AllAttendances()
        {
            var attendances = await data.Attendances
                .Select(a => new AttendanceViewModel
                {
                    Id = a.Id,
                    Subject = a.Subject.Name,
                    StudentFirstName = a.Student.FirstName,
                    StudentLastName = a.Student.LastName,
                    Status = a.Status,
                    DateAndTime = a.DateAndTime,
                    TeacherFirstName = a.Teacher.FirstName,
                    TeacherLastName = a.Teacher.LastName
                })
                .ToListAsync();

            return View(attendances);
        }

        public async Task<IActionResult> EditAttendance(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await data.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            var students = await data.Students.ToListAsync();
            var subjects = await data.Subjects.ToListAsync();

            var viewModel = new AttendanceFormViewModel
            {
                SelectedStudentId = attendance.StudentId,
                SelectedSubjectId = attendance.SubjectId,
                Status = attendance.Status,
                DateAndTime = attendance.DateAndTime,
                StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList(),
                SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList(),
                StatusOptions = Enum.GetValues(typeof(AttendanceStatus))
                    .Cast<AttendanceStatus>()
                    .Select(status => new SelectListItem
                    {
                        Value = ((int)status).ToString(),
                        Text = status.ToString()
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendance(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var students = await data.Students.ToListAsync();
                var subjects = await data.Subjects.ToListAsync();

                viewModel.StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList();

                viewModel.SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

                viewModel.StatusOptions = Enum.GetValues(typeof(AttendanceStatus))
                    .Cast<AttendanceStatus>()
                    .Select(status => new SelectListItem
                    {
                        Value = ((int)status).ToString(),
                        Text = status.ToString()
                    }).ToList();

                return View(viewModel);
            }

            var attendance = await data.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            attendance.StudentId = viewModel.SelectedStudentId;
            attendance.SubjectId = viewModel.SelectedSubjectId;
            attendance.Status = viewModel.Status;

            data.Update(attendance);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(AllAttendances));
        }

        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await data.Grades
                .Include(g => g.Student)
                .Include(g => g.Teacher)
                .Include(g => g.Subject)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                return NotFound();
            }

            var viewModel = new GradeViewModel
            {
                Id = grade.Id,
                Subject = grade.Subject.Name,
                StudentFirstName = grade.Student.FirstName,
                StudentLastName = grade.Student.LastName,
                TeacherFirstName = grade.Teacher.FirstName,
                TeacherLastName = grade.Teacher.LastName,
                Value = grade.Value,
                DateAndTime = grade.DateAndTime
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await data.Attendances
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            var viewModel = new AttendanceViewModel
            {
                Id = attendance.Id,
                StudentFirstName = attendance.Student.FirstName,
                StudentLastName = attendance.Student.LastName,
                TeacherFirstName = attendance.Teacher.FirstName,
                TeacherLastName = attendance.Teacher.LastName,
                Status = attendance.Status,
                DateAndTime = attendance.DateAndTime
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string entityType)
        {
            switch (entityType)
            {
                case "Grade":
                    var grade = await data.Grades.FindAsync(id);
                    if (grade == null)
                    {
                        return NotFound();
                    }
                    data.Grades.Remove(grade);
                    await data.SaveChangesAsync();
                    return RedirectToAction(nameof(AllGrades));

                case "Attendance":
                    var attendance = await data.Attendances.FindAsync(id);
                    if (attendance == null)
                    {
                        return NotFound();
                    }
                    data.Attendances.Remove(attendance);
                    await data.SaveChangesAsync();
                    return RedirectToAction(nameof(AllAttendances));

                default:
                    return BadRequest("Invalid entity type");
            }
        }
    }
}
