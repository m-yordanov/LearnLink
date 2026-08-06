using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LearnLink.Core.Constants.RoleConstants;

namespace LearnLink.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = TeacherRole)]
    public class TeacherBaseController : Controller
    {

    }
}
