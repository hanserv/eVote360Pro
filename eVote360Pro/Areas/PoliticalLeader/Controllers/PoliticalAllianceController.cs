using eVote360Pro.Core.Application.DTOs.PoliticalAlliance;
using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.PoliticalAlliance;
using eVote360Pro.Core.Application.ViewModels.User;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.PoliticalLeader.Controllers
{
    [Area("PoliticalLeader")]
    [SessionAuthorize(UserRole.PoliticalLeader)]
    public class PoliticalAllianceController : Controller
    {
        private readonly IPoliticalAllianceService _politicalAllianceService;
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;
        private readonly IPoliticalPartyService _politicalPartyService;
        public PoliticalAllianceController(IPoliticalAllianceService politicalAllianceService, IElectionService electionService,
            IMapper mapper, IPoliticalPartyService politicalPartyService)
        {
            _politicalAllianceService = politicalAllianceService;
            _electionService = electionService;
            _mapper = mapper;
            _politicalPartyService = politicalPartyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PendingRequests()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.GetPendingAlliancesAsync(currentUser!.Id);
            var vms = _mapper.Map<IEnumerable<PoliticalAllianceViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> RequestsMade()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.GetSentAlliancesAsync(currentUser!.Id);
            var vms = _mapper.Map<IEnumerable<PoliticalAllianceViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            await SetViewBagsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePoliticalAllianceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await SetViewBagsAsync();
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.CreateAllianceRequestAsync(_mapper.Map<CreatePoliticalAllianceDto>(vm), currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                await SetViewBagsAsync();
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "RequestsMade" });
        }

        public async Task<IActionResult> CurrentAlliances()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.GetCurrentAlliancesAsync(currentUser!.Id);

            var vms = _mapper.Map<IEnumerable<CurrentAllianceViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> Accept(int id)
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var result = await _politicalAllianceService.GetByIdAsync(id,currentUser!.Id);
            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "PendingRequests" });
            }
            return View(_mapper.Map<AcceptAllianceViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Accept(AcceptAllianceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.AcceptAllianceRequestAsync(vm.Id,currentUser!.Id);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "PendingRequests" });
        }

        public async Task<IActionResult> Reject(int id)
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var result = await _politicalAllianceService.GetByIdAsync(id, currentUser!.Id);
            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "PendingRequests" });
            }
            return View(_mapper.Map<RejectAllianceViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(RejectAllianceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.RejectAllianceRequestAsync(vm.Id, currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "PendingRequests" });
        }

        public async Task<IActionResult> DeleteRequest(int id)
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var result = await _politicalAllianceService.GetByIdAsync(id, currentUser!.Id);
            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "RequestsMade" });
            }
            return View(_mapper.Map<DeleteAllianceRequestViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequest(DeleteAllianceRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.DeleteAllianceRequestAsync(vm.Id, currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "RequestsMade" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var result = await _politicalAllianceService.GetByIdAsync(id, currentUser!.Id);
            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "CurrentAlliances" });
            }
            return View(_mapper.Map<DeleteAllianceViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAllianceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var currentUser = HttpContext.Session.Get<UserViewModel>("User");

            var result = await _politicalAllianceService.DeleteAllianceAsync(vm.Id, currentUser!.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "PoliticalLeader", controller = "PoliticalAlliance", action = "CurrentAlliances" });
        }

        #region Private methods
        private async Task SetViewBagsAsync()
        {
            var currentUser = HttpContext.Session.Get<UserViewModel>("User");
            var availablePartiesResult = await _politicalPartyService.GetAvailablePartiesForAllianceAsync(currentUser!.Id);

            ViewBag.PoliticalParties = availablePartiesResult.Value;
        }
        #endregion
    }
}
