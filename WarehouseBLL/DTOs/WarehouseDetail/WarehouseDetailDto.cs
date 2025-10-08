namespace WarehouseBLL.DTOs.WarehouseDetail;

public class WarehouseDetailDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}