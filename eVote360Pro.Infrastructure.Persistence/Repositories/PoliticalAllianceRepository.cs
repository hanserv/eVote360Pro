using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class PoliticalAllianceRepository : GenericRepository<PoliticalAlliance>, IPoliticalAllianceRepository
    {
        public PoliticalAllianceRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> HasActiveAllianceBetweenPartiesAsync(int partyAId, int partyBId)
        {
            return await _context.PoliticalAlliances
                            .AnyAsync(pa => ((pa.RequesterPartyId == partyAId && pa.TargetPartyId == partyBId) ||
                                             (pa.RequesterPartyId == partyBId && pa.TargetPartyId == partyAId)) && 
                                             pa.Status == AllianceStatus.Accepted);
        }

        public async Task<bool> HasPendingRequestAsync(int requesterPartyId, int targetPartyId)
        {
            return await _context.PoliticalAlliances
                            .AnyAsync(pa => pa.RequesterPartyId == requesterPartyId && 
                            pa.TargetPartyId == targetPartyId && 
                            pa.Status == AllianceStatus.Pending);
        }

        public async Task<int> CountAcceptedAlliancesAsync(int partyId)
        {
            return await _dbSet.CountAsync(pa => pa.Status == AllianceStatus.Accepted &&
                (pa.RequesterPartyId == partyId || pa.TargetPartyId == partyId));
        }

        public async Task<int> CountPendingReceivedRequestsAsync(int partyId)
        {
            return await _dbSet.CountAsync(pa => pa.Status == AllianceStatus.Pending &&
                             pa.TargetPartyId == partyId);
        }
    }
}
