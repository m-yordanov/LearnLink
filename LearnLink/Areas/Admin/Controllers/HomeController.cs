using Microsoft.AspNetCore.Mvc;

namespace LearnLink.Areas.Admin.Controllers
{
	public class HomeController : AdminBaseController
	{
		public IActionResult Dashboard()
		{
			return View();
		}
	}
}
