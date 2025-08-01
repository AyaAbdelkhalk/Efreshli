using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Data
{
    public class EfreshliDbContext : IdentityDbContext<ApplicationUser>
    {

        public EfreshliDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfreshliDbContext).Assembly);
            modelBuilder.ApplyGlobalFilters();
            modelBuilder.RestrictRelation();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            if (this.ChangeTracker.HasChanges())
            {
                this.UpdateAuditFields();
            }
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (this.ChangeTracker.HasChanges())
            {
                this.UpdateAuditFields();
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        
    }
}
