using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MarketplaceDAL.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IDbConnection _connection;

        public OrderItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(OrderItem entity, IDbTransaction? transaction = null)
        {
            var sql = @"INSERT INTO OrderItems 
                        (OrderId, ProductId, Quantity, UnitPrice, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            entity.OrderItemId = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: transaction);
        }

        public async Task UpdateAsync(OrderItem entity, IDbTransaction? transaction = null)
        {
            var sql = @"UPDATE OrderItems
                        SET OrderId = @OrderId,
                            ProductId = @ProductId,
                            Quantity = @Quantity,
                            UnitPrice = @UnitPrice,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE OrderItemId = @OrderItemId";

            await _connection.ExecuteAsync(sql, entity, transaction: transaction);
        }

        public async Task DeleteAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "UPDATE OrderItems SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE OrderItemId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow }, transaction: transaction);
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM OrderItems WHERE IsDeleted = 0";
            return await _connection.QueryAsync<OrderItem>(sql, transaction: transaction);
        }

        public async Task<OrderItem?> GetByIdAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM OrderItems WHERE OrderItemId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<OrderItem>(sql, new { Id = id }, transaction: transaction);
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM OrderItems WHERE OrderId = @OrderId AND IsDeleted = 0";
            return await _connection.QueryAsync<OrderItem>(sql, new { OrderId = orderId }, transaction: transaction);
        }

        public async Task<IEnumerable<OrderItem>> GetCreatedAfterAsync(DateTime date, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM OrderItems WHERE CreatedAt > @Date AND IsDeleted = 0";
            return await _connection.QueryAsync<OrderItem>(sql, new { Date = date }, transaction: transaction);
        }
        
        public async Task<OrderItem> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM OrderItems WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { Token = idempotencyToken });
        }
    }
}
