using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> HasParticipatedInElectionAsync(int id)
        {
            var isElectionCurrentlyActive = await _context.Elections
                .AnyAsync(e => e.Status == ElectionStatus.Active);

            if (isElectionCurrentlyActive)
            {
                var hasCurrentAssignments = await _context.PartyPositionAssignments
                            .AnyAsync(ppa => ppa.CandidateId == id);

                if (hasCurrentAssignments)
                {
                    return true;
                }
            }

            var hasReceivedVotes = await _context.Votes
                        .AnyAsync(v => v.CandidateId == id);

            return hasReceivedVotes;
        }

        public async Task<bool> IsAssignedToActiveElectivePositionAsync(int candidateId)
        {
            return await _context.PartyPositionAssignments
                            .AnyAsync(ppa => ppa.CandidateId == candidateId &&
                            ppa.ElectivePosition!.IsActive);
        }

        public async Task<IEnumerable<Candidate>> GetAvailableCandidatesForPartyAsync(int partyId)
        {
            var alliedPartyIds = await _context.PoliticalAlliances
                    .Where(pa => pa.Status == AllianceStatus.Accepted &&
                                 (pa.RequesterPartyId == partyId || pa.TargetPartyId == partyId) &&
                                 pa.RequesterParty!.IsActive && pa.TargetParty!.IsActive)
                    .Select(pa => pa.RequesterPartyId == partyId ? pa.TargetPartyId : pa.RequesterPartyId)
                    .ToListAsync();

            var activeCandidatesQuery = _dbSet
                    .Include(c => c.PoliticalParty)
                    .Where(c => c.IsActive && c.PoliticalParty!.IsActive);

            var ownCandidatesQuery = activeCandidatesQuery
                    .Where(c => c.PoliticalPartyId == partyId)
                    .Where(c => !_context.PartyPositionAssignments.Any(ppa => ppa.CandidateId == c.Id && ppa.AssigningPartyId == partyId));

            var alliedCandidatesQuery = activeCandidatesQuery
                    .Where(c => alliedPartyIds.Contains(c.PoliticalPartyId)) // son alia2
                    .Where(c => _context.PartyPositionAssignments.Any(ppa => ppa.CandidateId == c.Id && ppa.AssigningPartyId == c.PoliticalPartyId)) // tiene una asignacion en su partido de origen
                    .Where(c => !_context.PartyPositionAssignments.Any(ppa => ppa.CandidateId == c.Id && ppa.AssigningPartyId == partyId)); // no tienen ninguna asignacion en nuestro partido

            var availableCandidates = await ownCandidatesQuery.Concat(alliedCandidatesQuery)
                    .ToListAsync();

            return availableCandidates;
        }

        public async Task<int> CountCandidatesByStatusAsync(int partyId, bool isActive)
        {
            return await _dbSet.CountAsync(c => c.PoliticalPartyId == partyId && c.IsActive == isActive);
        }
    }
}
