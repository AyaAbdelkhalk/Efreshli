using Efreshli.Domain.Common.Interfaces;
using Efreshli.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly EfreshliDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(EfreshliDbContext context, DbSet<TEntity> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync([id], cancellationToken: cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return (await _dbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return _dbSet.Update(entity).Entity;
            }, cancellationToken);
        }

        public async Task<TEntity> RemoveAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            return _dbSet.Remove(entity).Entity;
        }

        public IQueryable<TEntity> GetByIdQueryable(int id)
        {
            var keyName = _context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties
                .Single().Name;

            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var property = Expression.Property(parameter, keyName);
            var value = Expression.Constant(id);
            var equals = Expression.Equal(property, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

            return _dbSet.Where(lambda);
        }

        public IQueryable<TEntity> GetByIdQueryable(TEntity entity)
        {
            var keyName = _context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties
                .Single().Name;

            var id = (int)typeof(TEntity).GetProperty(keyName).GetValue(entity);
            return GetByIdQueryable(id);
        }
    }
}
