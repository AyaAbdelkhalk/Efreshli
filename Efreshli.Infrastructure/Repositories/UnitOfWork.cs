using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EfreshliDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IGenericRepository<Category> CategoryRepository { get; private set; }
        public IGenericRepository<Image> ImageRepository { get; private set; }

        public UnitOfWork(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            CategoryRepository = new GenericRepository<Category>(_context, _httpContextAccessor);
            ImageRepository = new GenericRepository<Image>(_context, _httpContextAccessor);
        }








        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
        public void Dispose() => _context.Dispose();

       
    }
}
