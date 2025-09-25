using System.Data;
using System.Data.SqlClient;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class PaymentRepository : IPaymentRepository
{
    private readonly string _connectionString;

    public PaymentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // --- Generic CRUD ---

    public async Task AddAsync(Payment entity)
    {
        const string sql = @"INSERT INTO Payments 
                    (OrderId, PaymentDate, Amount, PaymentMethod, PaymentStatus, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@OrderId, @PaymentDate, @Amount, @PaymentMethod, @PaymentStatus, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@OrderId", entity.OrderId);
        cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
        cmd.Parameters.AddWithValue("@Amount", entity.Amount);
        cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod);
        cmd.Parameters.AddWithValue("@PaymentStatus", entity.PaymentStatus);
        cmd.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        cmd.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy);
        cmd.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);

        await conn.OpenAsync();
        entity.PaymentId = (long)(decimal)await cmd.ExecuteScalarAsync();
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = "UPDATE Payments SET IsDeleted = 1 WHERE PaymentId = @Id";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        const string sql = "SELECT * FROM Payments WHERE IsDeleted = 0";
        var payments = new List<Payment>();

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            payments.Add(MapPayment(reader));
        }

        return payments;
    }

    public async Task<Payment> GetByIdAsync(long id)
    {
        const string sql = "SELECT * FROM Payments WHERE PaymentId = @Id AND IsDeleted = 0";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapPayment(reader);
        }

        return null;
    }

    public async Task UpdateAsync(Payment entity)
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

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@OrderId", entity.OrderId);
        cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate);
        cmd.Parameters.AddWithValue("@Amount", entity.Amount);
        cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod);
        cmd.Parameters.AddWithValue("@PaymentStatus", entity.PaymentStatus);
        cmd.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PaymentId", entity.PaymentId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // --- Спеціальні методи ---

    public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(long orderId)
    {
        const string sql = "SELECT * FROM Payments WHERE OrderId = @OrderId AND IsDeleted = 0";
        var payments = new List<Payment>();

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            payments.Add(MapPayment(reader));
        }

        return payments;
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
    {
        const string sql = "SELECT * FROM Payments WHERE PaymentStatus = @Status AND IsDeleted = 0";
        var payments = new List<Payment>();

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Status", status);
        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            payments.Add(MapPayment(reader));
        }

        return payments;
    }

    public async Task<Payment> GetLatestPaymentForOrderAsync(long orderId)
    {
        const string sql = @"SELECT TOP 1 * FROM Payments 
                    WHERE OrderId = @OrderId AND IsDeleted = 0
                    ORDER BY PaymentDate DESC";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapPayment(reader);
        }

        return null;
    }

    // --- Helper: mapping ---
    private Payment MapPayment(SqlDataReader reader)
    {
        return new Payment
        {
            PaymentId = reader.GetInt64(reader.GetOrdinal("PaymentId")),
            OrderId = reader.GetInt64(reader.GetOrdinal("OrderId")),
            PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
            Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
            PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
            PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
            IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                ? null
                : reader.GetString(reader.GetOrdinal("UpdatedBy"))
        };
    }
}
