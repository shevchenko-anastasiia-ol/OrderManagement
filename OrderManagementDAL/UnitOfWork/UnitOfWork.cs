using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using MarketplaceDAL.Connection;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace MarketplaceDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConnectionFactory _connectionFactory;
        private IDbConnection? _connection;
        private IDbTransaction? _transaction;
        private bool _disposed = false;
        private readonly object _lockObject = new object();

        public ICustomerRepository CustomerRepository { get; private set;}
        public IOrderRepository OrderRepository { get; private set;}
        public IOrderItemRepository OrderItemRepository { get; private set;}
        public IProductRepository ProductRepository { get; private set;}
        public IPaymentRepository PaymentRepository { get; private set;}
        public IShipmentRepository ShipmentRepository { get; private set;}

        public UnitOfWork(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();

            InitializeRepositories();
        }
        
        private void InitializeRepositories()
        {
            if (_connection == null) throw new InvalidOperationException("Connection is null");

            CustomerRepository = new CustomerRepository(_connection, _transaction);
            OrderRepository = new OrderRepository(_connection, _transaction);
            OrderItemRepository = new OrderItemRepository(_connection, _transaction);
            ProductRepository = new ProductRepository(_connection, _transaction);
            PaymentRepository = new PaymentRepository(_connection, _transaction);
            ShipmentRepository = new ShipmentRepository(_connection, _transaction);
        }

        private void EnsureTransactionStarted()
        {
            lock (_lockObject)
            {
                if (_transaction == null)
                {
                    if (_connection?.State != ConnectionState.Open)
                        _connection?.Open();

                    _transaction = _connection!.BeginTransaction();
                    InitializeRepositories();
                }
            }
        }
        
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("Транзакція вже активна.");

            EnsureTransactionStarted();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            lock (_lockObject)
            {
                EnsureTransactionStarted();

                if (_transaction == null)
                    throw new InvalidOperationException("No active transaction");

                try
                {
                    _transaction.Commit();
                }
                catch
                {
                    _transaction.Rollback();
                    throw;
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            lock (_lockObject)
            {
                if (_transaction == null)
                    return Task.CompletedTask;

                try
                {
                    _transaction.Rollback();
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            return Task.CompletedTask;
        }
        

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_transaction != null)
            {
                await RollbackAsync();
            }

            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            _disposed = true;
        }
    }
}
