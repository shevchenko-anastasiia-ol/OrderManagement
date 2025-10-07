using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace MarketplaceDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public ICustomerRepository CustomerRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderItemRepository OrderItemRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public IShipmentRepository ShipmentRepository { get; }

        public UnitOfWork(IDbConnection connection,
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            IPaymentRepository paymentRepository,
            IShipmentRepository shipmentRepository)
        {
            _connection = connection;

            CustomerRepository = customerRepository;
            OrderRepository = orderRepository;
            OrderItemRepository = orderItemRepository;
            ProductRepository = productRepository;
            PaymentRepository = paymentRepository;
            ShipmentRepository = shipmentRepository;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("Транзакція вже активна.");

            if (_connection.State != ConnectionState.Open)
                await (_connection as SqlConnection)!.OpenAsync(cancellationToken);

            _transaction = _connection.BeginTransaction();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public IDbConnection Connection => _connection;
        public IDbTransaction? Transaction => _transaction;

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
