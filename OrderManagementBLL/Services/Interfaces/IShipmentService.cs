using OrderManagementBLL.DTOs.Shipment;

namespace OrderManagementBLL.Services.Interfaces;

public interface IShipmentService
{
    Task<IEnumerable<ShipmentDto>> GetAllShipmentsAsync();
    Task<ShipmentDto> GetShipmentByIdAsync(long id);
    Task<ShipmentDto> AddShipmentAsync(ShipmentCreateDto dto, string createdBy);
    Task<ShipmentDto> UpdateShipmentAsync(ShipmentUpdateDto dto, string updatedBy);
    Task DeleteShipmentAsync(long id);
    
    Task<IEnumerable<ShipmentDto>> GetShipmentsByOrderIdAsync(long orderId);
    Task<IEnumerable<ShipmentDto>> GetShipmentsByStatusAsync(string status);
    Task<ShipmentDto> GetLatestShipmentForOrderAsync(long orderId);
    Task<int> CountShipmentsByCarrierAsync(string carrier);
    Task<List<string>> GetDistinctCarriersAsync();
}