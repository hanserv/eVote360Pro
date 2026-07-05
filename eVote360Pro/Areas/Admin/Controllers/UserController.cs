using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IElectionService _electionService;

        public UserController(IUserService userService, IMapper mapper, 
            IElectionService electionService)
        {
            _userService = userService;
            _mapper = mapper;
            _electionService = electionService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _userService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<UserViewModel>>(result.Value!);
            return View(vms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _userService.AddAsync(_mapper.Map<CreateUserDto>(vm));

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "User", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "User", action = "Index" });
            }

            return View(_mapper.Map<UpdateUserViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = _mapper.Map<UpdateUserDto>(vm);

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _userService.UpdateAsync(dto,currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "User", action = "Index" });
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "User", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusUserViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _userService.ChangeStatusAsync(vm.Id, vm.NewStatus,currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "User", action = "Index" });
        }
    }
}
