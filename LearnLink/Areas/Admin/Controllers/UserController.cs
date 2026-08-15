using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;
using static LearnLink.Core.Constants.PaginationConstants;

namespace LearnLink.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        private readonly IUserService userService;
        private readonly IViewCommonService viewCommonService;

        public UserController(IUserService _userService, IViewCommonService _viewCommonService)
        {
            userService = _userService;
            viewCommonService = _viewCommonService;
        }

        public async Task<IActionResult> All(string searchString, int page = 1, int pageSize = maxPerPage)
        {
            pageSize = ClampPageSize(pageSize);

            var totalUsersCount = await userService.GetTotalUsersCountAsync(searchString);

            var totalPages = viewCommonService.CalculateTotalPages(totalUsersCount, pageSize);

            page = ClampToLastPage(page, totalPages);

            var users = await userService.GetFilteredUsersAsync(searchString, page, pageSize);

            var viewModel = new UserListViewModel
            {
                Users = users,
                RoleOptions = await userService.GetAllRolesAsync(),
                SearchString = searchString,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalUsersCount,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new UserFormViewModel
            {
                RoleOptions = await userService.GetAllRolesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData[UserMessageError] = "Failed to add the user!";
                viewModel.RoleOptions = await userService.GetAllRolesAsync();

                return View(viewModel);
            }

            var result = await userService.CreateUserAsync(viewModel);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData[UserMessageError] = "Failed to add the user!";
                viewModel.RoleOptions = await userService.GetAllRolesAsync();

                return View(viewModel);
            }

            TempData[UserMessageSuccess] = "You have added the user!";
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                TempData[UserMessageError] = "Please select a role!";
                ModelState.AddModelError("roleName", "Please select a role.");
                return RedirectToAction(nameof(All));
            }

            var success = await userService.ChangeUserRoleAsync(userId, roleName);

            if (!success)
            {
                return NotFound();
            }

			TempData[UserMessageSuccess] = "You have edited the role!";
			return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is required.");
            }

            var success = await userService.UnassignRoleAsync(userId);
            if (!success)
            {
                return NotFound();
            }

            TempData["UserMessageSuccess"] = "Role successfully unassigned.";
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(string userId, bool isActive)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is required.");
            }

            if (userId == GetCurrentUserId())
            {
                TempData[UserMessageError] = "You cannot deactivate your own account!";
                return RedirectToAction(nameof(All));
            }

            var success = await userService.SetUserActiveAsync(userId, isActive);

            if (!success)
            {
                return NotFound();
            }

            TempData[UserMessageSuccess] = isActive
                ? "You have activated the user!"
                : "You have deactivated the user!";

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is required.");
            }

            if (id == GetCurrentUserId())
            {
                TempData[UserMessageError] = "You cannot delete your own account!";
                return RedirectToAction(nameof(All));
            }

            var viewModel = await userService.GetUserForDeleteAsync(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User Id is required.");
            }

            if (id == GetCurrentUserId())
            {
                TempData[UserMessageError] = "You cannot delete your own account!";
                return RedirectToAction(nameof(All));
            }

            var result = await userService.DeleteUserAsync(id);

            switch (result)
            {
                case UserDeleteResult.UserNotFound:
                    return NotFound();

                case UserDeleteResult.LastAdmin:
                    TempData[UserMessageError] = "You cannot delete the last administrator!";
                    return RedirectToAction(nameof(All));

                case UserDeleteResult.Failed:
                    TempData[UserMessageError] = "Failed to delete the user!";
                    return RedirectToAction(nameof(All));

                default:
                    TempData[UserMessageSuccess] = "You have deleted the user!";
                    return RedirectToAction(nameof(All));
            }
        }

        private string? GetCurrentUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
