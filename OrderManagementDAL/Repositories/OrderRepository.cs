using System.Data;
using System.Data.SqlClient;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;
    private readonly IDbConnection _connection;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _connection = new SqlConnection(_connectionString);
    }

    // --- Generic CRUD (Dapper) ---

    public async Task AddAsync(Order entity)
    {
        var sql = @"INSERT INTO Orders (CustomerId, OrderDate, Status, TotalAmount, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@CustomerId, @OrderDate, @Status, @TotalAmount, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        entity.OrderId = await _connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task DeleteAsync(long id)
    {
        var sql = "UPDATE Orders SET IsDeleted = 1 WHERE OrderId = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        var sql = "SELECT * FROM Orders WHERE IsDeleted = 0";
        return await _connection.QueryAsync<Order>(sql);
    }

    public async Task<Order> GetByIdAsync(long id)
    {
        var sql = "SELECT * FROM Orders WHERE OrderId = @Id AND IsDeleted = 0";
        return await _connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Order entity)
    {
        var sql = @"UPDATE Orders
                    SET CustomerId = @CustomerId,
                        OrderDate = @OrderDate,
                        Status = @Status,
                        TotalAmount = @TotalAmount,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE OrderId = @OrderId";

        await _connection.ExecuteAsync(sql, entity);
    }

    // --- Спеціальні методи (Dapper) ---
    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId)
    {
        var sql = "SELECT * FROM Orders WHERE CustomerId = @CustomerId AND IsDeleted = 0";
        return await _connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
    {
        var sql = "SELECT * FROM Orders WHERE Status = @Status AND IsDeleted = 0";
        return await _connection.QueryAsync<Order>(sql, new { Status = status });
    }

    // --- Читання зі складними звʼязками (ADO.NET) ---
    public async Task<Order> GetOrderWithDetailsAsync(long orderId)
    {
        var sql = @"
            SELECT o.*, 
                   oi.OrderItemId, oi.ProductId, oi.Quantity, oi.UnitPrice,
                   p.PaymentId, p.Amount, p.PaymentDate, p.PaymentMethod, p.PaymentStatus,
                   s.ShipmentId, s.ShipmentDate, s.TrackingNumber, s.Carrier, s.Status, s.AddressLine1, s.AddressLine2, s.City, s.Region, s.PostalCode, s.Country
            FROM Orders o
            LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId
            LEFT JOIN Payments p ON o.OrderId = p.OrderId
            LEFT JOIN Shipments s ON o.OrderId = s.OrderId
            WHERE o.OrderId = @OrderId AND o.IsDeleted = 0";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@OrderId", orderId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        Order order = null;

        var orderItems = new List<OrderItem>();
        var payments = new List<Payment>();
        var shipments = new List<Shipment>();

        while (await reader.ReadAsync())
        {
            if (order == null)
            {
                order = new Order
                {
                    OrderId = reader.GetInt64(reader.GetOrdinal("OrderId")),
                    CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId")),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderItems = new List<OrderItem>(),
                    Payments = new List<Payment>(),
                    Shipments = new List<Shipment>()
                };
            }

            // OrderItem
            if (!reader.IsDBNull(reader.GetOrdinal("OrderItemId")))
            {
                var oi = new OrderItem
                {
                    OrderItemId = reader.GetInt64(reader.GetOrdinal("OrderItemId")),
                    ProductId = reader.GetInt64(reader.GetOrdinal("ProductId")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                };
                if (!order.OrderItems.Any(x => x.OrderItemId == oi.OrderItemId))
                    order.OrderItems.Add(oi);
            }

            // Payment
            if (!reader.IsDBNull(reader.GetOrdinal("PaymentId")))
            {
                var p = new Payment
                {
                    PaymentId = reader.GetInt64(reader.GetOrdinal("PaymentId")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                    PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                    PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus"))
                };
                if (!order.Payments.Any(x => x.PaymentId == p.PaymentId))
                    order.Payments.Add(p);
            }

            // Shipment
            if (!reader.IsDBNull(reader.GetOrdinal("ShipmentId")))
            {
                var s = new Shipment
                {
                    ShipmentId = reader.GetInt64(reader.GetOrdinal("ShipmentId")),
                    ShipmentDate = reader.GetDateTime(reader.GetOrdinal("ShipmentDate")),
                    TrackingNumber = reader.GetString(reader.GetOrdinal("TrackingNumber")),
                    Carrier = reader.GetString(reader.GetOrdinal("Carrier")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                    AddressLine2 = reader.IsDBNull(reader.GetOrdinal("AddressLine2")) ? null : reader.GetString(reader.GetOrdinal("AddressLine2")),
                    City = reader.GetString(reader.GetOrdinal("City")),
                    Region = reader.GetString(reader.GetOrdinal("Region")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    Country = reader.GetString(reader.GetOrdinal("Country"))
                };
                if (!order.Shipments.Any(x => x.ShipmentId == s.ShipmentId))
                    order.Shipments.Add(s);
            }
        }

        return order;
    }
}
