using System.Data;
using System.Data.SqlClient;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class ShipmentRepository : IShipmentRepository
{
    private readonly string _connectionString;

    public ShipmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // 🔹 Отримуємо з’єднання для Dapper
    private IDbConnection GetConnection() => new SqlConnection(_connectionString);

    // --- DAPPER CRUD ---
    public async Task AddAsync(Shipment entity)
    {
        using var connection = GetConnection();
        var sql = @"INSERT INTO Shipments 
                    (OrderId, ShipmentDate, TrackingNumber, Carrier, Status, AddressLine1, AddressLine2, City, Region, PostalCode, Country, CreatedAt, CreatedBy, IsDeleted)
                    VALUES (@OrderId, @ShipmentDate, @TrackingNumber, @Carrier, @Status, @AddressLine1, @AddressLine2, @City, @Region, @PostalCode, @Country, @CreatedAt, @CreatedBy, @IsDeleted);
                    SELECT CAST(SCOPE_IDENTITY() as bigint);";

        entity.ShipmentId = await connection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Shipments WHERE IsDeleted = 0";
        return await connection.QueryAsync<Shipment>(sql);
    }

    public async Task<Shipment> GetByIdAsync(long id)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Shipments WHERE ShipmentId = @Id AND IsDeleted = 0";
        return await connection.QueryFirstOrDefaultAsync<Shipment>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Shipment entity)
    {
        using var connection = GetConnection();
        var sql = @"UPDATE Shipments
                    SET OrderId = @OrderId,
                        ShipmentDate = @ShipmentDate,
                        TrackingNumber = @TrackingNumber,
                        Carrier = @Carrier,
                        Status = @Status,
                        AddressLine1 = @AddressLine1,
                        AddressLine2 = @AddressLine2,
                        City = @City,
                        Region = @Region,
                        PostalCode = @PostalCode,
                        Country = @Country,
                        UpdatedAt = @UpdatedAt,
                        UpdatedBy = @UpdatedBy
                    WHERE ShipmentId = @ShipmentId";

        await connection.ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(long id)
    {
        using var connection = GetConnection();
        var sql = "UPDATE Shipments SET IsDeleted = 1 WHERE ShipmentId = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    // --- DAPPER спец-методи ---
    public async Task<IEnumerable<Shipment>> GetShipmentsByOrderIdAsync(long orderId)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Shipments WHERE OrderId = @OrderId AND IsDeleted = 0";
        return await connection.QueryAsync<Shipment>(sql, new { OrderId = orderId });
    }

    public async Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Shipments WHERE Status = @Status AND IsDeleted = 0";
        return await connection.QueryAsync<Shipment>(sql, new { Status = status });
    }

    public async Task<Shipment> GetLatestShipmentForOrderAsync(long orderId)
    {
        using var connection = GetConnection();
        var sql = @"SELECT TOP 1 * FROM Shipments 
                    WHERE OrderId = @OrderId AND IsDeleted = 0
                    ORDER BY ShipmentDate DESC";

        return await connection.QueryFirstOrDefaultAsync<Shipment>(sql, new { OrderId = orderId });
    }

    // --- ADO.NET ПРИКЛАД ---
    public async Task<int> CountShipmentsByCarrierAsync(string carrier)
    {
        // Чистий ADO.NET — ручний SqlConnection + SqlCommand
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "SELECT COUNT(*) FROM Shipments WHERE Carrier = @Carrier AND IsDeleted = 0", 
            connection);

        command.Parameters.AddWithValue("@Carrier", carrier);

        var count = (int)await command.ExecuteScalarAsync();
        return count;
    }

    // --- ADO.NET приклад DataReader ---
    public async Task<List<string>> GetDistinctCarriersAsync()
    {
        var carriers = new List<string>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "SELECT DISTINCT Carrier FROM Shipments WHERE IsDeleted = 0", 
            connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            carriers.Add(reader.GetString(0));
        }

        return carriers;
    }
}
