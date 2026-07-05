using eVote360Pro.Core.Application.DTOs.Election;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.Election;
using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SessionAuthorize(UserRole.Admin)]
    public class ElectionController : Controller
    {
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;

        public ElectionController(IElectionService electionService, IMapper mapper)
        {
            _electionService = electionService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HasActiveElection = await _electionService.HasActiveElectionAsync();

            var result = await _electionService.GetAllElectionsAsync();
            var vms = _mapper.Map<IEnumerable<ElectionViewModel>>(result.Value!);

            return View(vms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateElectionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electionService.AddAsync(_mapper.Map<CreateElectionDto>(vm));

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" } );
        }

        public async Task<IActionResult> Activate(int id)
        {
            var result = await _electionService.GetByIdAsync(id);
            
            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusElectionViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(ChangeStatusElectionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var result = await _electionService.ActivateAsync(vm.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" });
        }

        public async Task<IActionResult> Finalize(int id)
        {
            var result = await _electionService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" });
            }

            return View(_mapper.Map<ChangeStatusElectionViewModel>(result.Value!));
        }

        [HttpPost]
        public async Task<IActionResult> Finalize(ChangeStatusElectionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electionService.FinalizeAsync(vm.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" });
        }

        public async Task<IActionResult> Result(int id)
        {
            var result = await _electionService.GetElectionResultsAsync(id);

            if(!result.IsSuccess)
            {
                return RedirectToRoute(new { area = "Admin", controller = "Election", action = "Index" });
            }

            var vms = _mapper.Map<ElectionResultsViewModel>(result.Value!);

            return View(vms);
        }

        public async Task<IActionResult> Summary()
        {
            var result = await _electionService.GetAvailableElectionYearsAsync();
            var availableYears = !result.IsSuccess ? [] : result.Value!.ToList();
            ViewBag.AvailableYears = availableYears;

            var vm = new ElectionSummaryPageViewModel();

            if (availableYears.Any())
            {
                vm.SelectedYear = availableYears.First();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Summary(ElectionSummaryPageViewModel vm)
        {
            var yearResult = await _electionService.GetAvailableElectionYearsAsync();
            ViewBag.AvailableYears = !yearResult.IsSuccess ? [] : yearResult.Value!.ToList();

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _electionService.GetElectoralSummaryByYearAsync(vm.SelectedYear);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            vm.Summaries = _mapper.Map<IEnumerable<ElectionSummaryViewModel>>(result.Value!);

            return View(vm);
        }
    }
}
