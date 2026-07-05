using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.DTOs.PoliticalParty;
using eVote360Pro.Core.Application.DTOs.User;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.Services;
using eVote360Pro.Core.Application.ViewModels.PoliticalParty;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using eVote360Pro.Helpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class PoliticalPartyController : Controller
    {
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;
        public PoliticalPartyController(IPoliticalPartyService politicalPartyService, IElectionService electionService,
            IMapper mapper)
        {
            _politicalPartyService = politicalPartyService;
            _electionService = electionService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _politicalPartyService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<PoliticalPartyViewModel>>(result.Value!);
            return View(vms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePoliticalPartyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var fileValidationResult = FileManager.ValidateFile(vm.LogoFile);

            if(!fileValidationResult.IsSuccess)
            {
                ModelState.AddModelError("", fileValidationResult.Error!);
                return View(vm);
            }

            var result = await _politicalPartyService.AddAsync(_mapper.Map<CreatePoliticalPartyDto>(vm));

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            result.Value!.Logo = FileManager.UploadAsync(vm.LogoFile,result.Value.Id, "PoliticalParties") ?? "";

            await _politicalPartyService.UpdateAsync(_mapper.Map<UpdatePoliticalPartyDto>(result.Value));

            return RedirectToRoute(new { area = "Admin", controller = "PoliticalParty", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _politicalPartyService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "PoliticalParty", action = "Index" });
            }

            return View(_mapper.Map<UpdatePoliticalPartyViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePoliticalPartyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var getResult = await _politicalPartyService.GetByIdAsync(vm.Id);

            if (!getResult.IsSuccess)
            {
                ModelState.AddModelError("", getResult.Error!);
                return View(vm);
            }

            var dto = _mapper.Map<UpdatePoliticalPartyDto>(vm);

            dto.Logo = FileManager.UploadAsync(vm.LogoFile, dto.Id, "PoliticalParties", isEditMode: true, dto.Logo);
            var result = await _politicalPartyService.UpdateAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "PoliticalParty", action = "Index" });
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _politicalPartyService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "PoliticalParty", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusPoliticalPartyViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusPoliticalPartyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _politicalPartyService.ChangeStatusAsync(vm.Id, vm.NewStatus);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "PoliticalParty", action = "Index" });
        }
    }
}
