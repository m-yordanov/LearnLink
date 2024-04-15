using LearnLink.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnLink.Areas.Teacher.Controllers
{
    public class HomeController : TeacherBaseController
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService _homeService)
        {
            homeService = _homeService;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = await homeService.GetTeacherHomeViewModelAsync(userId);

            if (viewModel != null)
            {
                return View(viewModel);
            }

            return NotFound();
        }
    }
}
