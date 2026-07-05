using eVote360Pro.Core.Domain.Interfaces;
using eVote360Pro.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360Pro.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        protected readonly eVote360ProContext _context;
        protected readonly DbSet<Entity> _dbSet;

        public GenericRepository(eVote360ProContext context)
        {
            _context = context;
            _dbSet = _context.Set<Entity>();
        }

        public virtual async Task<Entity> AddAsync(Entity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<Entity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual IQueryable<Entity> GetAllQuery()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<Entity>> GetAllListInclude(List<string> properties)
        {
            var query = _dbSet.AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }

        public IQueryable<Entity> GetAllQueryInclude(List<string> properties)
        {
            var query = _dbSet.AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return query;
        }

        public virtual async Task<Entity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task UpdateAsync(Entity entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
