using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LearnLink.Core.Constants.RoleConstants;

namespace LearnLink.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = StudentRole)]
    public class StudentBaseController : Controller
    {

    }
}
