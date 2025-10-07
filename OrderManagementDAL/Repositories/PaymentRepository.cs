using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MarketplaceDAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _connection;

        public PaymentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(Payment entity, IDbTransaction? transaction = null)
        {
            const string sql = @"INSERT INTO Payments 
                        (OrderId, PaymentDate, Amount, PaymentMethod, PaymentStatus, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@OrderId, @PaymentDate, @Amount, @PaymentMethod, @PaymentStatus, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            entity.PaymentId = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: transaction);
        }

        public async Task UpdateAsync(Payment entity, IDbTransaction? transaction = null)
        {
            const string sql = @"UPDATE Payments
                        SET OrderId = @OrderId,
                            PaymentDate = @PaymentDate,
                            Amount = @Amount,
                            PaymentMethod = @PaymentMethod,
                            PaymentStatus = @PaymentStatus,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE PaymentId = @PaymentId";

            await _connection.ExecuteAsync(sql, entity, transaction: transaction);
        }

        public async Task DeleteAsync(long id, IDbTransaction? transaction = null)
        {
            const string sql = "UPDATE Payments SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE PaymentId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow }, transaction: transaction);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(IDbTransaction? transaction = null)
        {
            const string sql = "SELECT * FROM Payments WHERE IsDeleted = 0";
            return await _connection.QueryAsync<Payment>(sql, transaction: transaction);
        }

        public async Task<Payment?> GetByIdAsync(long id, IDbTransaction? transaction = null)
        {
            const string sql = "SELECT * FROM Payments WHERE PaymentId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<Payment>(sql, new { Id = id }, transaction: transaction);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(long orderId, IDbTransaction? transaction = null)
        {
            const string sql = "SELECT * FROM Payments WHERE OrderId = @OrderId AND IsDeleted = 0";
            return await _connection.QueryAsync<Payment>(sql, new { OrderId = orderId }, transaction: transaction);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status, IDbTransaction? transaction = null)
        {
            const string sql = "SELECT * FROM Payments WHERE PaymentStatus = @Status AND IsDeleted = 0";
            return await _connection.QueryAsync<Payment>(sql, new { Status = status }, transaction: transaction);
        }

        public async Task<Payment> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM Payments WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Token = idempotencyToken });
        }

        public async Task<Payment?> GetLatestPaymentForOrderAsync(long orderId, IDbTransaction? transaction = null)
        {
            const string sql = @"SELECT TOP 1 * FROM Payments 
                        WHERE OrderId = @OrderId AND IsDeleted = 0
                        ORDER BY PaymentDate DESC";

            return await _connection.QuerySingleOrDefaultAsync<Payment>(sql, new { OrderId = orderId }, transaction: transaction);
        }
    }
}
