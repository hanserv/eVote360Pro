using System.ComponentModel.DataAnnotations;

namespace eVote360Pro.Core.Application.ViewModels.VoterIdentity
{
    public class CodeVerificationViewModel
    {
        public required string VerificationCode { get; set; }
    }
}
