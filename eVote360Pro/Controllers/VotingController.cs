using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.Voting;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Controllers
{
    [RedirectIfAuthenticated]
    [RequireVoterAuthentication]
    public class VotingController : Controller
    {
        private readonly IVotingService _votingService;
        private readonly IMapper _mapper;

        public VotingController(IVotingService votingService, IMapper mapper)
        {
            _votingService = votingService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");

            var result = await _votingService.GetAvailablePositionsForCitizenAsync(documentId!);
            
            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Cancel" });
            }

            var vms = _mapper.Map<IEnumerable<AvailablePositionViewModel>>(result.Value!);

            return View(vms);
        }

        public async Task<IActionResult> Candidates(int positionId)
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");

            var result = await _votingService.GetCandidatesForPositionAsync(positionId, documentId!);

            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "", controller = "Voting", action = "Index" });
            }

            var vms = _mapper.Map<PositionCandidatesViewModel>(result.Value!);
            return View(vms);
        }

        [HttpPost]
        public async Task<IActionResult> Vote(SaveVoteViewModel vm)
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");

            if (!ModelState.IsValid)
            {
                return await ReloadCandidatesViewAsync(vm.PositionId, documentId!);
            }

            var result = await _votingService.VoteAsync(vm.PositionId, vm.SelectedAssignmentId, documentId!);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return await ReloadCandidatesViewAsync(vm.PositionId, documentId!);
            }

            return RedirectToRoute(new { area = "", controller = "Voting", action = "Index" });
        }

        public IActionResult Finalize()
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");
            return View(new FinalizeVotingViewModel { DocumentId = documentId! });
        }

        [HttpPost]
        public async Task<IActionResult> Finalize(FinalizeVotingViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _votingService.FinalizeVotingAsync(vm.DocumentId);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Cancel" });
        }


        #region Private Methods
        private async Task<IActionResult> ReloadCandidatesViewAsync(int positionId, string documentId)
        {
            var reloadResult = await _votingService.GetCandidatesForPositionAsync(positionId, documentId);

            if (reloadResult.IsSuccess)
            {
                var vm = _mapper.Map<PositionCandidatesViewModel>(reloadResult.Value!);
                return View("Candidates", vm);
            }

            return RedirectToRoute(new { area = "", controller = "Voting", action = "Index" });
        }
        #endregion
    }
}
