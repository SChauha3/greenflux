using SmartCharging.Api.Models;
using System.Linq.Expressions;

namespace SmartCharging.Api.Repositories
{
    public interface IRepository<T>
    {
        Task<T?> FindAsync(Guid id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task RemoveAsync(T entity);
        Task SaveChangesAsync(T entity);
        Task UpdateChangesAsync(T entity);
    }

}
