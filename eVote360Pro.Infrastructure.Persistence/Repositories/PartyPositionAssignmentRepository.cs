using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class PartyPositionAssignmentRepository : GenericRepository<PartyPositionAssignment>, IPartyPositionAssignmentRepository
    {
        public PartyPositionAssignmentRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> HasAlliedCandidatesAssignedAsync(int partyAId, int partyBId)
        {
            return await _dbSet.Include(ppa => ppa.Candidate)
                            .AnyAsync(ppa => (ppa.AssigningPartyId == partyAId && ppa.Candidate!.PoliticalPartyId == partyBId) ||
                            (ppa.AssigningPartyId == partyBId && ppa.Candidate!.PoliticalPartyId == partyAId));
        }

        public async Task<bool> HasAssignmentInPartyAsync(int candidateId, int partyId)
        {
            return await _dbSet.AnyAsync(ppa => ppa.CandidateId == candidateId && ppa.AssigningPartyId == partyId);
        }

        public async Task<bool> IsPositionFilledInPartyAsync(int positionId, int partyId)
        {
            return await _dbSet.AnyAsync(ppa => ppa.ElectivePositionId == positionId && ppa.AssigningPartyId == partyId);
        }

        public async Task<int?> GetAssignedPositionInPartyAsync(int candidateId, int partyId)
        {
            var assignment = await _dbSet
                .FirstOrDefaultAsync(ppa => ppa.CandidateId == candidateId && ppa.AssigningPartyId == partyId);

            return assignment?.ElectivePositionId;
        }

        public async Task<int> CountAssignmentsByPartyAsync(int partyId)
        {
            return await _dbSet.CountAsync(ppa => ppa.AssigningPartyId == partyId);
        }

        public async Task<List<PartyPositionAssignment>> GetAllActiveAssignmentsAsync()
        {
            return await _dbSet.Include(ppa => ppa.Candidate)
                    .Where(ppa => ppa.Candidate != null && ppa.Candidate.IsActive)
                    .ToListAsync();
        }
    }
}
