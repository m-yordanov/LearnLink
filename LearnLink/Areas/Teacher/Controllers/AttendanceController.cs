using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class AttendanceController : TeacherBaseController
    {
        private readonly IAttendanceListService attendanceListService;
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceManagementService _attendanceManagementService, IViewCommonService _viewCommonService, IAttendanceListService _attendanceListService)
        {
            attendanceManagementService = _attendanceManagementService;
            viewCommonService = _viewCommonService;
            attendanceListService = _attendanceListService;
        }
        public async Task<IActionResult> All(AttendanceFilterModel filter)
        {
            this.WarnAboutIgnoredFilters();

            return View(await attendanceListService.BuildAsync(filter));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AttendanceFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
                viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();
                return View(viewModel);
			}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
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
