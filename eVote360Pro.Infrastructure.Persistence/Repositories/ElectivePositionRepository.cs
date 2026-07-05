using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class ElectivePositionRepository : GenericRepository<ElectivePosition>, IElectivePositionRepository
    {
        public ElectivePositionRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ElectivePosition>> GetAllActiveAsync()
        {
            return await _dbSet.Where(ep => ep.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<ElectivePosition>> GetAvailablePositionsForPartyAsync(int partyId)
        {
            return await _dbSet
                    .Where(ep => ep.IsActive)
                    .Where(ep => !_context.PartyPositionAssignments.Any(ppa => ppa.ElectivePositionId == ep.Id && ppa.AssigningPartyId == partyId)) 
                    .OrderBy(ep => ep.Name)
                    .ToListAsync();
        }
    }
}
