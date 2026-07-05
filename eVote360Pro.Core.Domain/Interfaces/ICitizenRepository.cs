using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface ICitizenRepository : IGenericRepository<Citizen>
    {
        Task<bool> HasVotedAsync(int id);
        Task<bool> IsDocumentIdAvailableAsync(string documentId, int? excludeId = null);
        Task<bool> IsEmailAvailableAsync(string email, int? excludeId = null);
        Task<Citizen?> GetByDocumentIdAsync(string documentId); 
    }
}
