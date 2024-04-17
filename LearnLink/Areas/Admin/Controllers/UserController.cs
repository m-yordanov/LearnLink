using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
    }
}
