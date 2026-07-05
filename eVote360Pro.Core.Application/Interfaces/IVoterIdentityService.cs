using eVote360Pro.Core.Application.DTOs.Citizen;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IVoterIdentityService
    {
        Task<Result> ProcessDocumentIdAndGenerateCodeAsync(string documentId, Stream imageStream, string fileName);
        Task<Result<CitizenDto>> ValidateDocumentAsync(string documentId);
        Task<Result> ValidateVerificationCodeAsync(string documentId, string code);
    }
}
