using System.Data;
using Dapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly IDbConnection _connection;

        public ShipmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task AddAsync(Shipment entity, IDbTransaction? transaction = null)
        {
            var sql = @"INSERT INTO Shipments 
                        (OrderId, ShipmentDate, TrackingNumber, Carrier, Status, AddressLine1, AddressLine2, City, Region, PostalCode, Country, CreatedAt, CreatedBy, IsDeleted)
                        VALUES (@OrderId, @ShipmentDate, @TrackingNumber, @Carrier, @Status, @AddressLine1, @AddressLine2, @City, @Region, @PostalCode, @Country, @CreatedAt, @CreatedBy, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() as bigint);";

            entity.ShipmentId = await _connection.ExecuteScalarAsync<long>(sql, entity, transaction: transaction);
        }

        public async Task UpdateAsync(Shipment entity, IDbTransaction? transaction = null)
        {
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

            await _connection.ExecuteAsync(sql, entity, transaction: transaction);
        }

        public async Task DeleteAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "UPDATE Shipments SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE ShipmentId = @Id";
            await _connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = System.DateTime.UtcNow }, transaction: transaction);
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Shipments WHERE IsDeleted = 0";
            return await _connection.QueryAsync<Shipment>(sql, transaction: transaction);
        }

        public async Task<Shipment?> GetByIdAsync(long id, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Shipments WHERE ShipmentId = @Id AND IsDeleted = 0";
            return await _connection.QuerySingleOrDefaultAsync<Shipment>(sql, new { Id = id }, transaction: transaction);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByOrderIdAsync(long orderId, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Shipments WHERE OrderId = @OrderId AND IsDeleted = 0";
            return await _connection.QueryAsync<Shipment>(sql, new { OrderId = orderId }, transaction: transaction);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status, IDbTransaction? transaction = null)
        {
            var sql = "SELECT * FROM Shipments WHERE Status = @Status AND IsDeleted = 0";
            return await _connection.QueryAsync<Shipment>(sql, new { Status = status }, transaction: transaction);
        }

        public async Task<Shipment?> GetLatestShipmentForOrderAsync(long orderId, IDbTransaction? transaction = null)
        {
            var sql = @"SELECT TOP 1 * FROM Shipments 
                        WHERE OrderId = @OrderId AND IsDeleted = 0
                        ORDER BY ShipmentDate DESC";

            return await _connection.QuerySingleOrDefaultAsync<Shipment>(sql, new { OrderId = orderId }, transaction: transaction);
        }
        public async Task<Shipment> GetByIdempotencyTokenAsync(string idempotencyToken)
        {
            var sql = "SELECT * FROM Shipments WHERE IdempotencyToken = @Token AND IsDeleted = 0";
            return await _connection.QueryFirstOrDefaultAsync<Shipment>(sql, new { Token = idempotencyToken });
        }

        public async Task<int> CountShipmentsByCarrierAsync(string carrier, IDbTransaction? transaction = null)
        {
            var sql = "SELECT COUNT(*) FROM Shipments WHERE Carrier = @Carrier AND IsDeleted = 0";
            return await _connection.ExecuteScalarAsync<int>(sql, new { Carrier = carrier }, transaction: transaction);
        }

        public async Task<List<string>> GetDistinctCarriersAsync(IDbTransaction? transaction = null)
        {
            var sql = "SELECT DISTINCT Carrier FROM Shipments WHERE IsDeleted = 0";
            var result = await _connection.QueryAsync<string>(sql, transaction: transaction);
            return result.ToList();
        }
    }
}
