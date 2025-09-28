using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using OrderManagementDAL.Data;

namespace MarketplaceDAL.UnitOfWork;

public class UnitOfWork: IUnitOfWork
{
    private readonly OrderManagementDbContext _context;
    private IDbContextTransaction? _transaction;
    
    public ICustomerRepository CustomerRepository {get; private set;}
    public IOrderRepository OrderRepository {get; private set;}
    public IOrderItemRepository OrderItemRepository {get; private set;}
    public IProductRepository ProductRepository {get; private set;}
    public IPaymentRepository PaymentRepository {get; private set;}
    public IShipmentRepository ShipmentRepository {get; private set;}

    public UnitOfWork(OrderManagementDbContext context,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        IPaymentRepository paymentRepository,
        IShipmentRepository shipmentRepository)
    {
        _context = context;
        CustomerRepository = customerRepository;
        OrderRepository = orderRepository;
        OrderItemRepository = orderItemRepository;
        ProductRepository = productRepository;
        PaymentRepository = paymentRepository;
        ShipmentRepository = shipmentRepository;
    }
    
    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Транзакція вже активна. Завершіть поточну транзакцію перед початком нової.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}