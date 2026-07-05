using eVote360Pro.Core.Application.DTOs.ElectivePosition;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.ElectivePosition;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class ElectivePositionController : Controller
    {
        private readonly IElectivePositionService _electivePositionService;
        private readonly IMapper _mapper;
        private readonly IElectionService _electionService;
        public ElectivePositionController(IElectivePositionService electivePositionService, IMapper mapper,
            IElectionService electionService)
        {
            _electivePositionService = electivePositionService;
            _mapper = mapper;
            _electionService = electionService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _electivePositionService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<ElectivePositionViewModel>>(result.Value!);
            return View(vms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateElectivePositionViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electivePositionService.AddAsync(_mapper.Map<CreateElectivePositionDto>(vm));

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "ElectivePosition", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _electivePositionService.GetByIdAsync(id);

            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "ElectivePosition", action = "Index" });
            }

            return View(_mapper.Map<UpdateElectivePositionViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateElectivePositionViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electivePositionService.UpdateAsync(_mapper.Map<UpdateElectivePositionDto>(vm));

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "ElectivePosition", action = "Index" });
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _electivePositionService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "ElectivePosition", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusElectiveViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusElectiveViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electivePositionService.ChangeStatusAsync(vm.Id,vm.NewStatus);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "ElectivePosition", action = "Index" });
        }
    }
}
