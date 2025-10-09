using System.ComponentModel.DataAnnotations;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.DTOs.Warehouse;

namespace WarehouseBLL.DTOs.Inventory;

public class InventoryWithDetailsDto : InventoryDto
{
    [Required]
    public WarehouseDto Warehouse { get; set; } = null!;
    [Required]
    public ProductDto Product { get; set; } = null!;
}