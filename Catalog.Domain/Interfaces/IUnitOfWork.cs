using Catalog.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Catalog.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClientSessionHandle Session { get; }
    Task StartTransactionAsync();
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task AbortAsync(CancellationToken cancellationToken = default);
}