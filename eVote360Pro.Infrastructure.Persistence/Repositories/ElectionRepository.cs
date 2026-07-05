using eVote360Pro.Core.Domain.Common.Enums;
using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class ElectionRepository : GenericRepository<Election>, IElectionRepository
    {
        public ElectionRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<Election?> GetActiveElectionAsync()
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Status == ElectionStatus.Active);
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            return await _dbSet.AnyAsync(e => e.Status == ElectionStatus.Active);
        }

        public async Task<List<Election>> GetElectionsOrderedAsync()
        {
            return await _dbSet
                            .OrderByDescending(e => e.Status == ElectionStatus.Active)
                            .ThenByDescending(e => e.Date)
                            .ToListAsync();
        }
    }
}
