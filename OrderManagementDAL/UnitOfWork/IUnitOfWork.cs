using System.Data;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;

namespace MarketplaceDAL.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository CustomerRepository { get; }
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    IProductRepository ProductRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IShipmentRepository ShipmentRepository { get; }
    
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}