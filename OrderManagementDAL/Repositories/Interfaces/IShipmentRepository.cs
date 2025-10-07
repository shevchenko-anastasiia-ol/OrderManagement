using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IShipmentRepository
    {
        Task AddAsync(Shipment entity, IDbTransaction? transaction = null);
        Task UpdateAsync(Shipment entity, IDbTransaction? transaction = null);
        Task<Shipment?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<Shipment>> GetAllAsync(IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);

        Task<IEnumerable<Shipment>> GetShipmentsByOrderIdAsync(long orderId, IDbTransaction? transaction = null);
        Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status, IDbTransaction? transaction = null);
        Task<Shipment?> GetLatestShipmentForOrderAsync(long orderId, IDbTransaction? transaction = null);
        Task<int> CountShipmentsByCarrierAsync(string carrier, IDbTransaction? transaction = null);
        Task<List<string>> GetDistinctCarriersAsync(IDbTransaction? transaction = null);
        Task<Shipment> GetByIdempotencyTokenAsync(string idempotencyToken);
    }
}