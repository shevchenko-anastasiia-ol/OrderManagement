using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MarketplaceDAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnection _connection;
        protected readonly IDbTransaction? _transaction;

        public CustomerRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
            
        }

        public async Task AddAsync(Customer entity, CancellationToken ct = default)
        {
            var sql = @"INSERT INTO Customers (FirstName, LastName, Email, Phone, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@FirstName, @LastName, @Email, @Phone, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            var id = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: _transaction);
            entity.CustomerId = id;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            var sql = "SELECT * FROM Customers WHERE IsDeleted = 0";
            return await _connection.QueryAsync<Customer>(sql, transaction: _transaction);
        }

        public async Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var sql = "SELECT * FROM Customers WHERE CustomerId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id }, transaction: _transaction);
        }

        public async Task UpdateAsync(Customer entity, CancellationToken ct = default)
        {
            var sql = @"UPDATE Customers
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            Email = @Email,
                            Phone = @Phone,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy,
                            IsDeleted = @IsDeleted
                        WHERE CustomerId = @CustomerId";

            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }
        
        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            var sql = "UPDATE Customers SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE CustomerId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow }, transaction: _transaction);
        }

        public async Task<IEnumerable<Customer>> GetCustomersCreatedAfterAsync(DateTime date, CancellationToken ct = default)
        {
            var sql = "SELECT * FROM Customers WHERE CreatedAt > @Date AND IsDeleted = 0";
            return await _connection.QueryAsync<Customer>(sql, new { Date = date }, transaction: _transaction);
        }
        
        public async Task<Customer> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM Customers WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Token = idempotencyToken });
        }

        public async Task<Customer> GetCustomerWithOrdersAsync(long customerId, CancellationToken ct = default)
        {
            var sql = @"
                SELECT c.*, o.OrderId, o.CustomerId AS OrderCustomerId, o.OrderDate, o.Status, o.TotalAmount
                FROM Customers c
                LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
                WHERE c.CustomerId = @CustomerId AND c.IsDeleted = 0";

            var lookup = new Dictionary<long, Customer>();

            var result = await _connection.QueryAsync<Customer, Order, Customer>(
                sql,
                (c, o) =>
                {
                    if (!lookup.TryGetValue(c.CustomerId, out var customer))
                    {
                        customer = c;
                        customer.Orders = new List<Order>();
                        lookup.Add(customer.CustomerId, customer);
                    }

                    if (o != null)
                        customer.Orders.Add(o);

                    return customer;
                },
                new { CustomerId = customerId },
                splitOn: "OrderId",
                transaction: _transaction
            );

            return lookup.Values.FirstOrDefault()!;
        }
    }
}
