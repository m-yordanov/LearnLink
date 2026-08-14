using LearnLink.Core.Models;
using LearnLink.Core.Interfaces;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;
using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
    public class AttendanceController : AdminBaseController
    {
        private readonly IAttendanceListService attendanceListService;
        private readonly IAttendanceManagementService attendanceManagementService;
        private readonly IViewCommonService viewCommonService;

        public AttendanceController(IAttendanceListService _attendanceListService, IAttendanceManagementService _AttendanceManagementService, IViewCommonService _viewCommonService)
        {
            attendanceListService = _attendanceListService;
            attendanceManagementService = _AttendanceManagementService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(AttendanceFilterModel filter)
        {
            return View(await attendanceListService.BuildAsync(filter));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await attendanceManagementService.GetAttendanceForEditAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendanceFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
				TempData[UserMessageError] = "Failed to edit the attendance!";
				viewModel.StudentOptions = (await viewCommonService.GetStudentOptionsAsync()).ToList();
                viewModel.SubjectOptions = (await viewCommonService.GetSubjectOptionsAsync()).ToList();

                return View(viewModel);
            }

            var result = await attendanceManagementService.UpdateAttendanceAsync(id, viewModel);

            if (!result)
            {
                return BadRequest();
            }

            TempData[UserMessageSuccess] = "You have edited the attendance!";
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await attendanceManagementService.GetAttendanceForDeleteAsync(id);
            
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await attendanceManagementService.DeleteAttendanceAsync(id);
            
            if (!result)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = "You have deleted the attendance!";
            return RedirectToAction(nameof(All));
        }
    }
}
