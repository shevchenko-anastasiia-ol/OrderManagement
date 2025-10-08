namespace WarehouseDomain.Entities;

public class WarehouseDetail
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string? Address { get; set; }
    public string? Manager { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
}