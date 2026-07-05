using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<int> CountCandidatesByStatusAsync(int partyId, bool isActive);
        Task<IEnumerable<Candidate>> GetAvailableCandidatesForPartyAsync(int partyId);
        Task<bool> HasParticipatedInElectionAsync(int id);
        Task<bool> IsAssignedToActiveElectivePositionAsync(int candidateId);
    }
}
