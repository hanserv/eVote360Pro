using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IPartyPositionAssignmentRepository : IGenericRepository<PartyPositionAssignment>
    {
        Task<int> CountAssignmentsByPartyAsync(int partyId);
        Task<int?> GetAssignedPositionInPartyAsync(int candidateId, int partyId);
        Task<bool> HasAlliedCandidatesAssignedAsync(int partyAId, int partyBId);
        Task<bool> HasAssignmentInPartyAsync(int candidateId, int partyId);
        Task<bool> IsPositionFilledInPartyAsync(int positionId, int partyId);
        Task<List<PartyPositionAssignment>> GetAllActiveAssignmentsAsync();
    }
}
