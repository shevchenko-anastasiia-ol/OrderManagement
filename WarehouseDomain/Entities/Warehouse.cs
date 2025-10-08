namespace WarehouseDomain.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<WarehouseDetail> Details { get; set; } = new List<WarehouseDetail>();
}