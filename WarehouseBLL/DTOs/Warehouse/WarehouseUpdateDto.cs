namespace WarehouseBLL.DTOs.Warehouse;

public class WarehouseUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int UpdatedBy { get; set; }
}