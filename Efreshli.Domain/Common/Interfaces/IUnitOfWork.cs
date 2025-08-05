using Efreshli.Domain.Models;

namespace Efreshli.Domain.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Image> ImageRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
