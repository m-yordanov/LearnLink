using LearnLink.Core.Interfaces;
using static LearnLink.Core.Constants.MessageConstants;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
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
    }
}
