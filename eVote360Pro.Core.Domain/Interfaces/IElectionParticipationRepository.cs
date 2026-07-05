using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IElectionParticipationRepository : IGenericRepository<ElectionParticipation>
    {
        Task<IEnumerable<ElectionParticipation>> GetByElectionIdAsync(int electionId);
        Task<bool> HasCitizenFinalizedAsync(int citizenId, int electionId);
    }
}
