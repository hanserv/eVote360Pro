using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public AuthController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [RedirectIfAuthenticated]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var result = await _userService.LoginAsync(_mapper.Map<LoginDto>(vm));

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            var userVm = _mapper.Map<UserViewModel>(result.Value!);
            HttpContext.Session.Set<UserViewModel>("User", userVm);

            return userVm.Role switch
            {
                UserRole.Admin => RedirectToRoute(new { area = "Admin", controller = "Home", action = "Index" }),
                UserRole.PoliticalLeader => RedirectToRoute(new { area = "PoliticalLeader", controller = "Home", action = "Index" }),
                _ => View()
            };
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("User");
            return RedirectToRoute(new { area = "", controller = "Auth", action = "Index" });
        }

        public IActionResult AccessDenied()
        {
            var userVm = HttpContext.Session.Get<UserViewModel>("User");
            return View(userVm);
        }
    }
}
