using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public IGenericRepository<Category> CategoryRepository { get; private set; }
        public IGenericRepository<Image> ImageRepository { get; private set; }

        public UnitOfWork(EfreshliDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            CategoryRepository = new GenericRepository<Category>(_context, _userManager);
            ImageRepository = new GenericRepository<Image>(_context, _userManager);
        }








        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
        public void Dispose() => _context.Dispose();

       
    }
}
