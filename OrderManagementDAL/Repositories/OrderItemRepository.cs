using System.Data;
using System.Data.SqlClient;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly string _connectionString;
    private readonly IDbConnection _connection;

    public OrderItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _connection = new SqlConnection(_connectionString);
    }

    // --- Generic CRUD (Dapper) ---

    public async Task AddAsync(OrderItem entity)
    {
        var sql = @"INSERT INTO OrderItems 
                    (OrderId, ProductId, Quantity, UnitPrice, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        entity.OrderItemId = await _connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task DeleteAsync(long id)
    {
        var sql = "UPDATE OrderItems SET IsDeleted = 1 WHERE OrderItemId = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        var sql = "SELECT * FROM OrderItems WHERE IsDeleted = 0";
        return await _connection.QueryAsync<OrderItem>(sql);
    }

    public async Task<OrderItem> GetByIdAsync(long id)
    {
        var sql = "SELECT * FROM OrderItems WHERE OrderItemId = @Id AND IsDeleted = 0";
        return await _connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { Id = id });
    }

    public async Task UpdateAsync(OrderItem entity)
    {
        var sql = @"UPDATE OrderItems
                    SET OrderId = @OrderId,
                        ProductId = @ProductId,
                        Quantity = @Quantity,
                        UnitPrice = @UnitPrice,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE OrderItemId = @OrderItemId";

        await _connection.ExecuteAsync(sql, entity);
    }

    // --- Спеціальні методи (ADO.NET) ---

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId)
    {
        var list = new List<OrderItem>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM OrderItems WHERE OrderId = @OrderId AND IsDeleted = 0", conn);
        cmd.Parameters.AddWithValue("@OrderId", orderId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var item = new OrderItem
            {
                OrderItemId = reader.GetInt64(reader.GetOrdinal("OrderItemId")),
                OrderId = reader.GetInt64(reader.GetOrdinal("OrderId")),
                ProductId = reader.GetInt64(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
            list.Add(item);
        }

        return list;
    }

    public async Task<IEnumerable<OrderItem>> GetCreatedAfterAsync(DateTime date)
    {
        var list = new List<OrderItem>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM OrderItems WHERE CreatedAt > @Date AND IsDeleted = 0", conn);
        cmd.Parameters.AddWithValue("@Date", date);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var item = new OrderItem
            {
                OrderItemId = reader.GetInt64(reader.GetOrdinal("OrderItemId")),
                OrderId = reader.GetInt64(reader.GetOrdinal("OrderId")),
                ProductId = reader.GetInt64(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
            list.Add(item);
        }

        return list;
    }
}
