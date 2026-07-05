using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.PoliticalLeader.Controllers
{
    [Area("PoliticalLeader")]
    [SessionAuthorize(UserRole.PoliticalLeader)]
    public class HomeController : Controller
    {
        private readonly IPoliticalLeaderDashboardService _politicalLeaderDashboardService;
        private readonly IMapper _mapper;

        public HomeController(IPoliticalLeaderDashboardService politicalLeaderDashboardService, IMapper mapper)
        {
            _politicalLeaderDashboardService = politicalLeaderDashboardService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            ViewBag.CurrentUserName = $"{currentUser!.Name} {currentUser.LastName}";

            var result = await _politicalLeaderDashboardService.GetDashboardDataAsync(currentUser.Id);

            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "", controller="Auth", action = "LogOut" });
            }

            var vm = _mapper.Map<PoliticalLeaderDashboardViewModel>(result.Value!);
            return View(vm);
        }
    }
}
