using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360Pro.Core.Application.ViewModels.VoterIdentity
{
    public class IdentityVerificationViewModel
    {
        public required IFormFile ImageFile { get; set; }
    }
}
