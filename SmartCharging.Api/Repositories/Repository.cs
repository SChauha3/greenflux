using Microsoft.EntityFrameworkCore;
using SmartCharging.Api.Data;
using System.Linq.Expressions;

namespace SmartCharging.Api.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<T?> FindAsync(Guid id)
        {
            return await _appDbContext.Set<T>().FindAsync(id);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _appDbContext.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includes = null)
        {
            IQueryable<T> query = _appDbContext.Set<T>();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            var entry = _appDbContext.Set<T>().Entry(entity);
            _appDbContext.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(T entity)
        {
            await _appDbContext.Set<T>().AddAsync(entity);
            // Save changes to the database.
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateChangesAsync(T entity)
        {
            var entry = _appDbContext.Set<T>().Entry(entity);
            entry.State = EntityState.Modified;
            // Save changes to the database.
            await _appDbContext.SaveChangesAsync();
        }
    }
}