using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IShipmentRepository : IGenericRepository<Shipment>
{
    Task<IEnumerable<Shipment>> GetShipmentsByOrderIdAsync(long orderId);
    
    Task<IEnumerable<Shipment>> GetShipmentsByStatusAsync(string status);
    
    Task<Shipment> GetLatestShipmentForOrderAsync(long orderId);
    Task<int> CountShipmentsByCarrierAsync(string carrier);
    Task<List<string>> GetDistinctCarriersAsync();
}