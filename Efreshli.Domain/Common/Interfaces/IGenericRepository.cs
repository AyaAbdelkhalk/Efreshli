using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Common.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> RemoveAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetByIdQueryable(int id);
        IQueryable<TEntity> GetByIdQueryable(TEntity entity);
        Task SaveChangesAsync();
    }
}
