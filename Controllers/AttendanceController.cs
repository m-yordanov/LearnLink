//using LearnLink.Data;
//using LearnLink.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;


//public class AttendanceController : Controller
//{
//    private readonly LearnLinkDbContext data;

//    public AttendanceController(LearnLinkDbContext context)
//    {
//        data = context;
//    }

//    public async Task<IActionResult> All()
//    {
//        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//        var attendances = await data.Attendances
//            .Include(a => a.Subject)
//            .Include (a => a.Teacher)
//            .Where(a => a.Student.UserId == studentId)
//            .ToListAsync();

//        var attendanceViewModels = attendances.Select(a => new AttendanceViewModel
//        {
//            DateAndTime = a.DateAndTime,
//            Status = a.Status,
//            Subject = a.Subject.Name,
//            TeacherFirstName = a.Teacher.FirstName,
//            TeacherLastName = a.Teacher.LastName
//        }).ToList();

//        return View(nameof(All), attendanceViewModels);
//    }
//}

