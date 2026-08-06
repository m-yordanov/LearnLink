using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LearnLink.Core.Constants.RoleConstants;

namespace LearnLink.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = AdminRole)]
	public class AdminBaseController : Controller
	{

	}
}
