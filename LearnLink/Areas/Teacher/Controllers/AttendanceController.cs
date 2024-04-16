using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using static LearnLink.Core.Constants.MessageConstants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Teacher.Controllers
{
	public class AttendanceController : TeacherBaseController
    {
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceManagementService _attendanceManagementService, IViewCommonService _viewCommonService)
        {
            attendanceManagementService = _attendanceManagementService;
            viewCommonService = _viewCommonService;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new AttendanceFormViewModel
            {
                StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList(),
                SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AttendanceFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
				TempData[UserMessageError] = "Failed to add the attendance!";

				if (viewModel.SelectedStudentId <= 0)
                {
                    ViewData["SelectedStudentIdValidationError"] = "Please select a student.";
                }

                if (viewModel.SelectedSubjectId <= 0)
                {
                    ViewData["SelectedSubjectIdValidationError"] = "Please select a subject.";
                }

                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
			}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await attendanceManagementService.AddAttendanceAsync(viewModel, userId);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to add attendance.");
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
            }

			TempData[UserMessageSuccess] = "You have added the attendance!";
			return RedirectToAction(nameof(Add));
        }
    }
}
