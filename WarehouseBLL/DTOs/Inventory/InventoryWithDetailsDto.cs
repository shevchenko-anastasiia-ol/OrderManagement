using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Warehouse;

namespace WarehouseBLL.DTOs.Inventory;

public class InventoryWithDetailsDto : InventoryDto
{
    public WarehouseDto Warehouse { get; set; } = null!;
    public ProductDto Product { get; set; } = null!;
}