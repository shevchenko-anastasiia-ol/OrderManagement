namespace WarehouseBLL.DTOs.Inventory;

public class InventoryUpdateDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int UpdatedBy { get; set; }
}