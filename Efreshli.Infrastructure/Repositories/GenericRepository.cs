using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly EfreshliDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenericRepository(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _httpContextAccessor = httpContextAccessor;
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
            //audit
            if (entity is IAuditable auditableEntity)
            {
                auditableEntity.CreatedDate = DateTime.UtcNow;
                var currentUrId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                auditableEntity.CreatedBy = currentUrId ?? "SysteminGeneric";

            }
            return (await _dbSet.AddAsync(entity, cancellationToken)).Entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is IAuditable auditableEntity)
            {
                auditableEntity.UpdatedDate = DateTime.UtcNow;
                var currentUrId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                auditableEntity.UpdatedBy = currentUrId ?? "SysteminGeneric";
            }
            _dbSet.Update(entity);
            return entity;
        }

        public async Task<TEntity> RemoveAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            // Check if the entity supports soft delete
            if(entity is IAuditable auditable)
            {
                auditable.DeletedDate = DateTime.UtcNow;
                var currentUrId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                auditable.DeletedBy = currentUrId ?? "SysteminGeneric";
                auditable.IsDeleted = true;
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

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync();

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }





        
        public async Task<TEntity?> GetByIdWithThenIncludeAsync(
            int id,
            CancellationToken cancellationToken = default,
            params string[] includes)
        {
            var query = BuildQueryWithStringIncludes(_dbSet, includes);

            var keyName = _context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties
                .Single().Name;

            return await query.FirstOrDefaultAsync(
                e => EF.Property<int>(e, keyName).Equals(id),
                cancellationToken);
        }





        public async Task<PaginatedResult<TEntity>> GetPagedAsync(int pageNumber = 1, int pageSize = 24, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            // Validate parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 24;
            if (pageSize > 100) pageSize = 100; // Prevent large page sizes

            // Build the query
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            // Apply includes first for better query planning
            query = BuildQueryWithIncludes(query, includes);

            // Apply filtering
            if (predicate != null)
                query = query.Where(predicate);

            // Get total count before pagination (with same filters)
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply ordering - default by primary key if not specified
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                // Default ordering by primary key for consistent pagination
                var keyName = _context.Model.FindEntityType(typeof(TEntity))
                    .FindPrimaryKey().Properties
                    .Single().Name;

                query = query.OrderBy(e => EF.Property<object>(e, keyName));
            }

            // Apply pagination
            var skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Execute query
            var items = await query.ToListAsync(cancellationToken);

            // Return paginated result
            return new PaginatedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber < Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = pageNumber > 1
            };
        }

        public async Task<PaginatedResult<TEntity>> GetPagedWithStringIncludesAsync(int pageNumber = 1, int pageSize = 24, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default, params string[] includes)
        {
            // Validate parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 24;
            if (pageSize > 100) pageSize = 100;

            // Build the query
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            // Apply string includes
            query = BuildQueryWithStringIncludes(query, includes);

            // Apply filtering
            if (predicate != null)
                query = query.Where(predicate);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                var keyName = _context.Model.FindEntityType(typeof(TEntity))
                    .FindPrimaryKey().Properties
                    .Single().Name;

                query = query.OrderBy(e => EF.Property<object>(e, keyName));
            }

            // Apply pagination
            var skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Execute query
            var items = await query.ToListAsync(cancellationToken);

            // Return result
            return new PaginatedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber < Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = pageNumber > 1
            };
        }

        public async Task<PaginatedResult<TResult>> GetPagedWithProjectionAsync<TResult>(Expression<Func<TEntity, TResult>> selector, int pageNumber = 1, int pageSize = 24, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 24;
            if (pageSize > 100) pageSize = 100;

            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            // Apply includes
            query = BuildQueryWithIncludes(query, includes);

            // Apply filtering
            if (predicate != null)
                query = query.Where(predicate);

            // Get count before projection
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                var keyName = _context.Model.FindEntityType(typeof(TEntity))
                    .FindPrimaryKey().Properties
                    .Single().Name;

                query = query.OrderBy(e => EF.Property<object>(e, keyName));
            }

            // Apply pagination and projection
            var skip = (pageNumber - 1) * pageSize;
            var projectedItems = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TResult>
            {
                Items = projectedItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber < Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = pageNumber > 1
            };
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
        private IQueryable<TEntity> BuildQueryWithStringIncludes(
    IQueryable<TEntity> query,
    params string[] includes)
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

        #endregion

    }
}
