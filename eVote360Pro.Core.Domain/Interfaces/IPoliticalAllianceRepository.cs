using eVote360Pro.Core.Domain.Entities;

namespace eVote360Pro.Core.Domain.Interfaces
{
    public interface IPoliticalAllianceRepository : IGenericRepository<PoliticalAlliance>
    {
        Task<int> CountAcceptedAlliancesAsync(int partyId);
        Task<int> CountPendingReceivedRequestsAsync(int partyId);
        Task<bool> HasActiveAllianceBetweenPartiesAsync(int partyAId, int partyBId);
        Task<bool> HasPendingRequestAsync(int requesterPartyId, int targetPartyId);
    }
}
