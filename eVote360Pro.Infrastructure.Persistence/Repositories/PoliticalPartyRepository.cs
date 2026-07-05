using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class PoliticalPartyRepository : GenericRepository<PoliticalParty>, IPoliticalPartyRepository
    {
        public PoliticalPartyRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> IsAcronymAvailableAsync(string acronym, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Acronym == acronym);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> HasParticipatedInElectionAsync(int id)
        {
            var isThereAnActiveElection = await _context.Elections
                .AnyAsync(e => e.Status == ElectionStatus.Active);

            if (isThereAnActiveElection)
            {
                var hasCurrentAssignments = await _context.PartyPositionAssignments
                            .AnyAsync(ppa => ppa.AssigningPartyId == id);

                if (hasCurrentAssignments)
                {
                    return true;
                }
            }

            var hasReceivedVotes = await _context.Votes
                .AnyAsync(v => v.Candidate != null && v.Candidate.PoliticalPartyId == id);

            return hasReceivedVotes;
        }

        public async Task<bool> HasActiveCandidatesAsync(int id)
        {
            return await _dbSet.AnyAsync(pp => pp.Id == id && pp.Candidates!.Any(c => c.IsActive));
        }

        public async Task<bool> HasAssignedLeaderAsync(int id, bool? isActive = null)
        {
            return await _dbSet.AnyAsync(pp => pp.Id == id && pp.User != null && (!isActive.HasValue || pp.User.IsActive == isActive.Value));
        }

        public async Task<IEnumerable<PoliticalParty>> GetAllActiveAsync()
        {
            return await _dbSet.Where(pp => pp.IsActive).ToListAsync();
        }
    }
}
