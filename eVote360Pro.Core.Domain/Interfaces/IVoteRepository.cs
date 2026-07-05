using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<IEnumerable<Vote>> GetVotesByElectionIdAsync(int electionId);
    }
}
