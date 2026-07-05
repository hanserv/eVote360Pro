using eVote360Pro.Core.Application.Helpers;
using eVote360Pro.Core.Application.Interfaces;
using eVote360Pro.Core.Application.ViewModels.VoterIdentity;
using eVote360Pro.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360Pro.Controllers
{
    [RedirectIfAuthenticated]
    [RedirectIfVoterAuthenticated]
    public class VoterIdentityController : Controller
    {
        private readonly IVoterIdentityService _voterIdentityService;

        public VoterIdentityController(IVoterIdentityService voterIdentityService)
        {
            _voterIdentityService = voterIdentityService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DocumentEntryViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _voterIdentityService.ValidateDocumentAsync(vm.DocumentId);

            if(!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            HttpContext.Session.Set<string>("VoterDocument", vm.DocumentId);

            return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "VerifyIdentity" });
        }

        public IActionResult VerifyIdentity()
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Index" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyIdentity(IdentityVerificationViewModel vm)
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");
            if (string.IsNullOrEmpty(documentId))
            {
                return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Index" });
            }

            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            using var stream = vm.ImageFile.OpenReadStream();
            var result = await _voterIdentityService.ProcessDocumentIdAndGenerateCodeAsync(documentId, stream, vm.ImageFile.FileName);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error!);
                return View(vm);
            }

            HttpContext.Session.Set<bool>("IsOcrValidated", true);
            return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "VerifyCode" });
        }

        public IActionResult VerifyCode()
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");
            var isOcrValidated = HttpContext.Session.Get<bool>("IsOcrValidated");

            if (string.IsNullOrEmpty(documentId) || !isOcrValidated)
            {
                return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Index" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(CodeVerificationViewModel vm)
        {
            var documentId = HttpContext.Session.Get<string>("VoterDocument");
            var isOcrValidated = HttpContext.Session.Get<bool>("IsOcrValidated");

            if (string.IsNullOrEmpty(documentId) || !isOcrValidated)
            {
                return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _voterIdentityService.ValidateVerificationCodeAsync(documentId, vm.VerificationCode);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("",result.Error!);
                return View(vm);
            }

            HttpContext.Session.Set<bool>("IsVoterAuthenticated", true);

            return RedirectToRoute(new { area = "", controller = "Voting", action = "Index" });
        }

        public IActionResult Cancel()
        {
            HttpContext.Session.Clear();
            return RedirectToRoute(new { area = "", controller = "VoterIdentity", action = "Index" });
        }
    }
}
