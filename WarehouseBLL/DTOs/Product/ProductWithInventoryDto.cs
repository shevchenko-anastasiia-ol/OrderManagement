using WarehouseBLL.DTOs.Inventory;

namespace WarehouseBLL.DTOs.Product;

public class ProductWithInventoryDto :  ProductDto
{
    public List<InventoryDto> Inventories { get; set; } = new();
}