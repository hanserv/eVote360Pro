using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IPoliticalPartyRepository : IGenericRepository<PoliticalParty>
    {
        Task<bool> HasParticipatedInElectionAsync(int id);
        Task<bool> IsAcronymAvailableAsync(string acronym, int? excludeId = null);
        Task<bool> HasActiveCandidatesAsync(int id);
        Task<bool> HasAssignedLeaderAsync(int id,bool? isActive = null);
        Task<IEnumerable<PoliticalParty>> GetAllActiveAsync();
    }
}
