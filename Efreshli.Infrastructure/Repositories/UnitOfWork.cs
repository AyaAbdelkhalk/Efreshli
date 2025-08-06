using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Efreshli.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

public class UnitOfWork : IUnitOfWork
{
    private readonly EfreshliDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IGenericRepository<Category> CategoryRepository { get; private set; }
    public IGenericRepository<Image> ImageRepository { get; private set; }
    public IGenericRepository<WebsiteInfo> WebsiteInfoRepository { get; private set; }

    public UnitOfWork(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        CategoryRepository = new GenericRepository<Category>(_context, _httpContextAccessor);
        ImageRepository = new GenericRepository<Image>(_context, _httpContextAccessor);
        WebsiteInfoRepository = new GenericRepository<WebsiteInfo>(_context, _httpContextAccessor);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose() => _context.Dispose();
}
