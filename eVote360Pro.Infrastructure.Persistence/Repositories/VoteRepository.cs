using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository
    {
        public VoteRepository(eVote360ProContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Vote>> GetVotesByElectionIdAsync(int electionId)
        {
            return await _dbSet
                        .Include(v => v.Candidate)
                        .Where(v => v.ElectionId == electionId)
                        .ToListAsync();
        }
    }
}
