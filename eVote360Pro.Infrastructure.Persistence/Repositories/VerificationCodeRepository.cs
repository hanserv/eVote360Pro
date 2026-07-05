using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class VerificationCodeRepository : GenericRepository<VerificationCode>, IVerificationCodeRepository
    {
        public VerificationCodeRepository(eVote360ProContext context) : base(context)
        {
        }

        public async Task<VerificationCode?> GetLatestCodeAsync(int citizenId, int electionId)
        {
            return await _dbSet.Where(vc => vc.CitizenId == citizenId && vc.ElectionId == electionId)
                            .OrderByDescending(vc => vc.GenerationDate)
                            .FirstOrDefaultAsync();
        }
    }
}
