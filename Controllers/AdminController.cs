﻿using LearnLink.Data;
using LearnLink.Data.Models;
using LearnLink.Data.Models.Enums;
using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Controllers
{
    public class AdminController : Controller
    {
        private readonly LearnLinkDbContext data;

        public AdminController(LearnLinkDbContext context)
        {
            data = context;
        }

        public async Task<IActionResult> AllGrades()
        {
            var grades = await data.Grades
                .Select(g => new GradeViewModel
                {
                    Id = g.Id,
                    Subject = g.Subject.Name,
                    StudentFirstName = g.Student.FirstName,
                    StudentLastName = g.Student.LastName,
                    Value = g.Value,
                    DateAndTime = g.DateAndTime,
                    TeacherFirstName = g.Teacher.FirstName,
                    TeacherLastName = g.Teacher.LastName
                })
                .ToListAsync();

            return View(grades);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await data.Grades.FindAsync(id);

            if (grade == null)
            {
                return NotFound();
            }

            data.Grades.Remove(grade);
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

    }
}
