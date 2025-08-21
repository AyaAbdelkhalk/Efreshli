using Microsoft.EntityFrameworkCore.Query;
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

        //multiple include
        Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(Expression<Func<TEntity, bool>>? predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdWithIncludeAsync(
            int id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);
        ////////////////////
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> RemoveAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<TEntity> GetByIdQueryable(int id);
        IQueryable<TEntity> GetByIdQueryable(TEntity entity);
        Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetWhereSingleAsync(Expression<Func<TEntity, bool>> expression);
        Task<IEnumerable<TResult>> GetWhereSelectAsync<TResult>(Expression<Func<TEntity, bool>> Condition, Expression<Func<TEntity, TResult>> expression);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);
        Task SaveChangesAsync();
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<TEntity> GetByIdWithThenIncludeAsync(
            int id,
            CancellationToken cancellationToken = default,
            params string[] includes);
    }
}
