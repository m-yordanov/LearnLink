using LearnLink.Data;
using LearnLink.Data.Models;
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
    }
}
