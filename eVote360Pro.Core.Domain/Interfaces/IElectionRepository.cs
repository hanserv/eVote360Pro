using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IElectionRepository : IGenericRepository<Election>
    {
        Task<bool> HasActiveElectionAsync();
        Task<Election?> GetActiveElectionAsync();
        Task<List<Election>> GetElectionsOrderedAsync();
    }
}
