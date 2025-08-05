using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public GenericRepository(EfreshliDbContext context, UserManager<ApplicationUser> userManager = null)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _userManager = userManager;
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
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
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

            // Check if entity supports soft delete (IsDeleted property)
            var isDeletedProp = typeof(TEntity).GetProperty("IsDeleted");
            if (isDeletedProp != null)
            {
                isDeletedProp.SetValue(entity, true);

                // Optionally set DeletedDate and DeletedBy if available
                var deletedDateProp = typeof(TEntity).GetProperty("DeletedDate");
                if (deletedDateProp != null)
                    deletedDateProp.SetValue(entity, DateTime.UtcNow);

                var deletedByProp = typeof(TEntity).GetProperty("DeletedBy");
                if (deletedByProp != null)
                    deletedByProp.SetValue(entity, "system"); // Replace with current user if available

                _dbSet.Update(entity);
                return entity;
            }

            // Fallback to hard delete if soft delete not supported
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

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
        public async Task<TEntity> GetWhereSingleAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.SingleOrDefaultAsync(expression);
        }
        public async Task<IEnumerable<TResult>> GetWhereSelectAsync<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> expression)
        {
            return await _dbSet.Where(condition).Select(expression).ToListAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.CountAsync(expression);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(
            Expression<Func<TEntity, bool>>? predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = BuildQueryWithIncludes(_dbSet, includes);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdWithIncludeAsync(
            int id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = BuildQueryWithIncludes(_dbSet, includes);
            var keyName = _context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties
                .Single().Name;
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, keyName).Equals(id), cancellationToken);
        }

        #region HelperMethods
        private IQueryable<TEntity> BuildQueryWithIncludes(
           IQueryable<TEntity> query,
           params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
        private IQueryable<TEntity> BuildQuery(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        #endregion

    }
}
