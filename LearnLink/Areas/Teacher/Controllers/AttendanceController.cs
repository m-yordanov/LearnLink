﻿using LearnLink.Core.Models;
using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Globalization;
using LearnLink.Infrastructure.Data.Models.Enums;

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

            if (TempData.ContainsKey("AttendanceAdded") && (bool)TempData["AttendanceAdded"])
            {
                viewModel.AttendanceAddedSuccessfully = true;
                TempData.Remove("AttendanceAdded");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AttendanceFormViewModel viewModel)
        {
			if (!ModelState.IsValid)
			{
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

            TempData["AttendanceAdded"] = true;
            return RedirectToAction(nameof(Add));
        }
    }
}
