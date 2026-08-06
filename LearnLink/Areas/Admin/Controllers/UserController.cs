using LearnLink.Core.Interfaces;
using LearnLink.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LearnLink.Core.Constants.MessageConstants;

namespace LearnLink.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        public async Task<IActionResult> All()
        {
            var usersWithRoles = await userService.GetAllUsersWithRolesAsync();
            var roles = await userService.GetAllRolesAsync();

            ViewData["Roles"] = roles;

            return View(usersWithRoles);
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
