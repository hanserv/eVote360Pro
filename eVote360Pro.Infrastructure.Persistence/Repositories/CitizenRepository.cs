using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class CitizenRepository : GenericRepository<Citizen>, ICitizenRepository
    {
        public CitizenRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<bool> HasVotedAsync(int id)
        {
            return await _context.ElectionParticipations.AnyAsync(ep => ep.CitizenId == id);
        }
        public async Task<bool> IsEmailAvailableAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Email == email);

            if(excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsDocumentIdAvailableAsync(string documentId, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.DocumentId == documentId);

            if(excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId);
            }

            return !await query.AnyAsync();
        }

        public async Task<Citizen?> GetByDocumentIdAsync(string documentId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.DocumentId == documentId);
        }
    }
}
