using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IVerificationCodeRepository : IGenericRepository<VerificationCode>
    {
        Task<VerificationCode?> GetLatestCodeAsync(int citizenId, int electionId);
    }
}
