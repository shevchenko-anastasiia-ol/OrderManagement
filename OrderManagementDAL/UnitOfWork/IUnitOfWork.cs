using System.Data;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;

namespace MarketplaceDAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    ICustomerRepository CustomerRepository { get; }
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    IProductRepository ProductRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IShipmentRepository ShipmentRepository { get; }
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

    
}