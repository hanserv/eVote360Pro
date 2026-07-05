using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            ViewBag.CurrentUserName = $"{currentUser!.Name} {currentUser.LastName}";
            return View();
        }
    }
}
