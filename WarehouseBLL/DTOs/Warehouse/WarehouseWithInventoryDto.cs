using WarehouseBLL.DTOs.Inventory;

namespace WarehouseBLL.DTOs.Warehouse;

public class WarehouseWithInventoryDto : WarehouseDto
{
    public List<InventoryDto> Inventories { get; set; } = new();
}