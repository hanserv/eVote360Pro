using eVote360Pro.Core.Domain.Entities;
using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(eVote360ProContext context) : base(context)
        {
        }

        public Task<User?> LoginAsync(string userName, string password)
        {
            return _dbSet.Include(u => u.PoliticalParty)
                            .FirstOrDefaultAsync(u => u.Username == userName && u.PasswordHash == password);
        }
    }
}
