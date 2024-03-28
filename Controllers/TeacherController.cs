using LearnLink.Data.Models;
using LearnLink.Data;
using LearnLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Controllers
{
    using LearnLink.Data;
    using LearnLink.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class TeacherController : Controller
    {
        private readonly LearnLinkDbContext data;

        public TeacherController(LearnLinkDbContext context)
        {
            data = context;
        }

        [HttpGet]
        public async Task<IActionResult> AddGrade()
        {
            var students = await data.Students.ToListAsync();
            var subjects = await data.Subjects.ToListAsync();

            var viewModel = new AddGradeViewModel
            {
                StudentOptions = students.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList(),
                SubjectOptions = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            if (TempData.ContainsKey("GradeAdded") && (bool)TempData["GradeAdded"])
            {
                viewModel.GradeAddedSuccessfully = true;

                TempData.Remove("GradeAdded");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(AddGradeViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            var teacherId = teacher.Id;

            //handle null teacher cases later

            if (!ModelState.IsValid)
            {
                viewModel.StudentOptions = await data.Students
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = $"{s.FirstName} {s.LastName}"
                    }).ToListAsync();
                viewModel.SubjectOptions = await data.Subjects
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync();
                return View(viewModel);
            }

            var subjectId = viewModel.SelectedSubjectId;
            var subject = await data.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var grade = new Grade
            {
                StudentId = viewModel.SelectedStudentId,
                Subject = subject,
                Value = viewModel.Grade,
                DateAndTime = DateTime.Now,
                TeacherId = teacher.Id
            };

            data.Grades.Add(grade);
            await data.SaveChangesAsync();

            TempData["GradeAdded"] = true;

            return RedirectToAction("AddGrade");
        }

    }
}
