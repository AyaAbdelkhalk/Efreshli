using Efreshli.Domain.Models;


namespace Efreshli.Domain.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Image> ImageRepository { get; }

        IGenericRepository<WebsiteInfo> WebsiteInfoRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
