using eVote360Pro.Core.Application.DTOs.Citizen;
using eVote360Pro.Core.Application.DTOs.PoliticalLeaderAssignment;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.Services;
using eVote360Pro.Core.Application.ViewModels.Citizen;
using eVote360Pro.Core.Application.ViewModels.PoliticalLeaderAssignment;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class PoliticalLeaderAssignment : Controller
    {
        private readonly IPoliticalLeaderAssignmentService _politicalLeaderAssignmentService;
        private readonly IUserService _userService;
        private readonly IPoliticalPartyService _politicalPartyService;
        private readonly IMapper _mapper;
        private readonly IElectionService _electionService;


        public PoliticalLeaderAssignment(IPoliticalLeaderAssignmentService politicalLeaderAssignmentService, IUserService userService,
            IPoliticalPartyService politicalPartyService, IMapper mapper, 
            IElectionService electionService)
        {
            _politicalLeaderAssignmentService = politicalLeaderAssignmentService;
            _userService = userService;
            _politicalPartyService = politicalPartyService;
            _mapper = mapper;
            _electionService = electionService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _politicalLeaderAssignmentService.GetAllAsync();
            var vms = _mapper.Map<IEnumerable<PoliticalLeaderAssignmentViewModel>>(result.Value!);
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            await SetViewBagsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePoliticalLeaderAssignmentViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                await SetViewBagsAsync();
                return View(vm);
            }

            var result = await _politicalLeaderAssignmentService.AsyncPoliticalLeaderAsync(_mapper.Map<CreatePoliticalLeaderAssignmentDto>(vm));

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                await SetViewBagsAsync();
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "PoliticalLeaderAssignment", action = "Index" });
        }

        public async Task<IActionResult> Delete(int userId,int politicalPartyId)
        {
            var result = await _politicalLeaderAssignmentService.ExistRelationAsync(userId,politicalPartyId);
            
            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "PoliticalLeaderAssignment", action = "Index" });
            }

            var vm = new DeleteAssignmentViewModel
            {
                Id = userId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssignmentViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _politicalLeaderAssignmentService.DeleteAssignmentAsync(vm.Id);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "PoliticalLeaderAssignment", action = "Index" });
        }

        #region Private Methods
        private async Task SetViewBagsAsync()
        {
            var availableUsersResult = await _userService.GetAllAvailableLeadersAsync();
            var availablePartiesResult = await _politicalPartyService.GetAllAvailablePartiesAsync();

            ViewBag.Users = availableUsersResult.Value;
            ViewBag.PoliticalParties = availablePartiesResult.Value;
        }
        #endregion
    }
}
