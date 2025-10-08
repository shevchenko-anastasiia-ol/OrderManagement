namespace WarehouseBLL.DTOs.WarehouseDetail;

public class WarehouseDetailUpdateDto
{
    public int Id { get; set; }
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public int UpdatedBy { get; set; }
}