using eVote360Pro.Core.Application.DTOs.Candidates;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.Candidate;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using eVote360Pro.Helpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.PoliticalLeader.Controllers
{
    [Area("PoliticalLeader")]
    [SessionAuthorize(UserRole.PoliticalLeader)]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public CandidateController(ICandidateService candidateService, IElectionService electionService,
            IMapper mapper)
        {
            _candidateService = candidateService;
            _electionService = electionService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _candidateService.GetAllAsync(currentUser!.Id);
            var vms = _mapper.Map<IEnumerable<CandidateViewModel>>(result.Value!);
            return View(vms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCandidateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var fileValidationResult = FileManager.ValidateFile(vm.PhotoFile);

            if (!fileValidationResult.IsSuccess)
            {
                ModelState.AddModelError("", fileValidationResult.Error!);
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _candidateService.AddAsync(_mapper.Map<CreateCandidateDto>(vm), currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            result.Value!.Photo = FileManager.UploadAsync(vm.PhotoFile, result.Value.Id, "Candidates") ?? "";

            await _candidateService.UpdateAsync(_mapper.Map<UpdateCandidateDto>(result.Value), currentUser!.Id);

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "Candidate", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _candidateService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "Candidate", action = "Index" });
            }

            return View(_mapper.Map<UpdateCandidateViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCandidateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var getResult = await _candidateService.GetByIdAsync(vm.Id);

            if (!getResult.IsSuccess)
            {
                ModelState.AddModelError("", getResult.Error!);
                return View(vm);
            }

            var dto = _mapper.Map<UpdateCandidateDto>(vm);

            dto.Photo = FileManager.UploadAsync(vm.PhotoFile, dto.Id, "Candidates", isEditMode: true, dto.Photo);

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _candidateService.UpdateAsync(dto, currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "Candidate", action = "Index" });
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _candidateService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "Candidate", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusCandidateViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusCandidateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _candidateService.ChangeStatusAsync(vm.Id, vm.NewStatus,currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "Candidate", action = "Index" });
        }
    }
}
