using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Data
{
    public static class EntityFrameworkExtensions
    {
        public static void UpdateAuditFields(this DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified));
            
            foreach (var entry in entries)
            {
                var auditableEntity = (IAuditable)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedBy = UserContext.CurrentUserId ?? "system";
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditableEntity.UpdatedBy = UserContext.CurrentUserId ?? "system";
                    auditableEntity.UpdatedDate = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    auditableEntity.IsDeleted = true;
                    auditableEntity.DeletedBy = UserContext.CurrentUserId ?? "system"; 
                    auditableEntity.DeletedDate = DateTime.UtcNow;
                }
            }
        }

        // Keep the old overload for backward compatibility if needed
        public static void UpdateAuditFields(this DbContext context, IUserContext userContext)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                var auditableEntity = (IAuditable)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedBy = userContext.CurrentUserId;
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditableEntity.UpdatedBy = userContext.CurrentUserId;
                    auditableEntity.UpdatedDate = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    auditableEntity.IsDeleted = true;
                    auditableEntity.DeletedBy = userContext.CurrentUserId; 
                    auditableEntity.DeletedDate = DateTime.UtcNow;
                }
            }
        }

        public static LambdaExpression GetIsDeletedFilter(Type type)
        {
            var param = Expression.Parameter(type, "e");
            var prop = Expression.Property(param, "IsDeleted");
            var constant = Expression.Constant(false);
            var body = Expression.Equal(prop, constant);
            return Expression.Lambda(body, param);
        }

        public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
                {
                    var filter = GetIsDeletedFilter(entityType.ClrType);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter((LambdaExpression)filter);
                }
            }
        }

        public static void RestrictRelation(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configure cascade delete behavior for each foreign key
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }
    }
}
