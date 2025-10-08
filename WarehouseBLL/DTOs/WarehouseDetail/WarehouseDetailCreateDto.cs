namespace WarehouseBLL.DTOs.WarehouseDetail;

public class WarehouseDetailCreateDto
{
    public int WarehouseId { get; set; }
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public int CreatedBy { get; set; }
}