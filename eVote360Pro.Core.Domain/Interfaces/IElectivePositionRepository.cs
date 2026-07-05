using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IElectivePositionRepository : IGenericRepository<ElectivePosition>
    {
        Task<IEnumerable<ElectivePosition>> GetAvailablePositionsForPartyAsync(int partyId);
        Task<IEnumerable<ElectivePosition>> GetAllActiveAsync();
    }
}
