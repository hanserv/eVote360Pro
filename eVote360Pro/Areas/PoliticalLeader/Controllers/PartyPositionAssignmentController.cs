using eVote360Pro.Core.Application.DTOs.PartyPositionAssignment;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.PartyPositionAssignment;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.PoliticalLeader.Controllers
{
    [Area("PoliticalLeader")]
    [SessionAuthorize(UserRole.PoliticalLeader)]
    public class PartyPositionAssignmentController : Controller
    {
        private readonly IPartyPositionAssignmentService _partyPositionAssignmentService;
        private readonly IMapper _mapper;
        private readonly IElectionService _electionService;

        public PartyPositionAssignmentController(IPartyPositionAssignmentService partyPositionAssignmentService, IMapper mapper, 
            IElectionService electionService)
        {
            _partyPositionAssignmentService = partyPositionAssignmentService;
            _mapper = mapper;
            _electionService = electionService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _partyPositionAssignmentService.GetAllSummaryAsync(currentUser!.Id);
            var vms = _mapper.Map<IEnumerable<AssignmentSummaryViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            await SetViewBagsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePartyPositionAssignmentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _partyPositionAssignmentService.AddAsync(_mapper.Map<CreatePartyPositionAssignmentDto>(vm), currentUser!.Id);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PartyPositionAssignment", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _partyPositionAssignmentService.GetByIdAsync(id, currentUser!.Id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "PartyPositionAssignment", action = "Index" });
            }

            var vm = new DeleteAssignmentViewModel
            {
                Id = result.Value!.Id
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssignmentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _partyPositionAssignmentService.DeleteAsync(vm.Id,currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PartyPositionAssignment", action = "Index" });
        }


        #region Private methods
        private async Task SetViewBagsAsync()
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var availableCandidatesResult = await _partyPositionAssignmentService.GetAvailableCandidatesForAssignmentAsync(currentUser!.Id);
            var availableElectivePositionsResult = await _partyPositionAssignmentService.GetAvailablePositionsForAssignmentAsync(currentUser!.Id);

            ViewBag.Candidates = availableCandidatesResult.Value;
            ViewBag.ElectivePositions = availableElectivePositionsResult.Value;
        }
        #endregion
    }
}
