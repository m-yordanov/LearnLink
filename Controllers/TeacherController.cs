using LearnLink.Models;
using LearnLink.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LearnLink.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IGradeManagementService gradeManagementService;
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public TeacherController(IGradeManagementService _gradeManagementService, IAttendanceManagementService _attendanceManagementService, IViewCommonService _viewCommonService)
        {
            gradeManagementService = _gradeManagementService;
            attendanceManagementService = _attendanceManagementService;
            viewCommonService = _viewCommonService;
        }

        [HttpGet]
        public async Task<IActionResult> AddGrade()
        {
            var viewModel = new GradeFormViewModel
            {
                StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList(),
                SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList()
            };


            if (TempData.ContainsKey("GradeAdded") && (bool)TempData["GradeAdded"])
            {
                viewModel.GradeAddedSuccessfully = true;
                TempData.Remove("GradeAdded");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade(GradeFormViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await gradeManagementService.AddGradeAsync(viewModel, userId);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to add grade.");
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
            }

            TempData["GradeAdded"] = true;
            return RedirectToAction("AddGrade");
        }

        [HttpGet]
        public async Task<IActionResult> AddAttendance()
        {
            var viewModel = new AttendanceFormViewModel
            {
                StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList(),
                SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList()
            };

            if (TempData.ContainsKey("AttendanceAdded") && (bool)TempData["AttendanceAdded"])
            {
                viewModel.AttendanceAddedSuccessfully = true;
                TempData.Remove("AttendanceAdded");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance(AttendanceFormViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await attendanceManagementService.AddAttendanceAsync(viewModel, userId);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to add attendance.");
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
            }

            TempData["AttendanceAdded"] = true;
            return RedirectToAction("AddAttendance");
        }
    }
}
