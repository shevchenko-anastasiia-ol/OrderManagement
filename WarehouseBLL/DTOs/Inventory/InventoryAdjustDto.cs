namespace WarehouseBLL.DTOs.Inventory;

public class InventoryAdjustDto
{
    public int Id { get; set; }
    public int QuantityChange { get; set; }
    public int UpdatedBy { get; set; }
}