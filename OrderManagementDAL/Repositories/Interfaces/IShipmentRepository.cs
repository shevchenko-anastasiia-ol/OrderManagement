using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IShipmentRepository
    {
        Task AddAsync(Shipment entity, CancellationToken ct = default);
        Task UpdateAsync(Shipment entity, CancellationToken ct = default);
        Task<Shipment?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<Shipment>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);

        Task<IEnumerable<Shipment>> GetShipmentsByOrderIdAsync(long orderId, CancellationToken ct = default);
        Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status, CancellationToken ct = default);
        Task<Shipment?> GetLatestShipmentForOrderAsync(long orderId, CancellationToken ct = default);
        Task<int> CountShipmentsByCarrierAsync(string carrier, CancellationToken ct = default);
        Task<List<string>> GetDistinctCarriersAsync(CancellationToken ct = default);
        Task<Shipment> GetByIdempotencyTokenAsync(string idempotencyToken);
    }
}