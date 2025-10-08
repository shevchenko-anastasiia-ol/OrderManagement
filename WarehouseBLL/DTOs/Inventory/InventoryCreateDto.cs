namespace WarehouseBLL.DTOs.Inventory;

public class InventoryCreateDto
{
    public int WarehouseId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int CreatedBy { get; set; }
}