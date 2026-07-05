using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.Services;
using eVote360Pro.Core.Application.ViewModels.Citizen;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class CitizenController : Controller
    {
        private readonly ICitizenService _citizenService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;
        public CitizenController(ICitizenService citizenService, IElectionService electionService, 
            IMapper mapper)
        {
            _citizenService = citizenService;
            _electionService = electionService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _citizenService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<CitizenViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCitizenViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _citizenService.AddAsync(_mapper.Map<CreateCitizenDto>(vm));

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Citizen", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _citizenService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "Citizen", action = "Index" });
            }

            return View(_mapper.Map<UpdateCitizenViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCitizenViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var dto = _mapper.Map<UpdateCitizenDto>(vm);

            var result = await _citizenService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Citizen", action = "Index" });
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _citizenService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "Citizen", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusCitizenViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusCitizenViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _citizenService.ChangeStatusAsync(vm.Id, vm.NewStatus);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Citizen", action = "Index" });
        }
    }
}
