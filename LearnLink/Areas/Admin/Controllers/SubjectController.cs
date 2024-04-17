using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;
using static LearnLink.Core.Constants.MessageConstants;

namespace LearnLink.Areas.Admin.Controllers
{
    public class SubjectController : AdminBaseController
    {
        private readonly ISubjectService subjectService;
        private readonly IViewCommonService viewCommonService;

        public SubjectController(ISubjectService _subjectService, IViewCommonService _viewCommonService)
        {
            subjectService = _subjectService;
            viewCommonService = _viewCommonService;
        }
        public async Task<IActionResult> All(string searchString, int page = 1, int pageSize = 10)
        {
            var subjects = await subjectService.GetFilteredSubjectsAsync(searchString, page, pageSize);

            var totalSubjectsCount = await subjectService.GetTotalSubjectsCountAsync(searchString);

            var totalPages = viewCommonService.CalculateTotalPages(totalSubjectsCount, pageSize);


            var viewModel = new SubjectViewModel
            {
                Subjects = subjects,
                SearchString = searchString,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalSubjectsCount,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string subjectName)
        {
            if (string.IsNullOrEmpty(subjectName))
            {
                TempData[UserMessageError] = "Subject name cannot be empty!";
                ModelState.AddModelError("subjectName", "Subject name cannot be empty.");
                return View();
            }

            bool isSubjectAdded = await subjectService.AddSubjectAsync(subjectName);

            if (!isSubjectAdded)
            {
                TempData[UserMessageError] = "A subject with this name already exists!";
                ModelState.AddModelError("subjectName", "A subject with this name already exists.");
                return View();
            }

            TempData[UserMessageSuccess] = "You added the subject successfully!";
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await subjectService.EditSubjectFormViewModelAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData[UserMessageError] = "Failed to update subject name!";
                return View(viewModel);
            }

            var result = await subjectService.UpdateSubjectAsync(id, viewModel.SubjectName);
            if (!result)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = "Subject name updated successfully!";
            return RedirectToAction(nameof(All));
        }
    }
}
