namespace WarehouseBLL.DTOs.Warehouse;

public class WarehouseCreateDto
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int CreatedBy { get; set; }
}