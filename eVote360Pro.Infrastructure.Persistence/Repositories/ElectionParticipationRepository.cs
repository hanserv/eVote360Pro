using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class ElectionParticipationRepository : GenericRepository<ElectionParticipation>, IElectionParticipationRepository
    {
        public ElectionParticipationRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> HasCitizenFinalizedAsync(int citizenId, int electionId)
        {
            return await _dbSet.AnyAsync(p => p.CitizenId == citizenId && p.ElectionId == electionId);
        }

        public async Task<IEnumerable<ElectionParticipation>> GetByElectionIdAsync(int electionId)
        {
            return await _dbSet
                        .Where(ep => ep.ElectionId == electionId)
                        .ToListAsync();
        }
    }
}
