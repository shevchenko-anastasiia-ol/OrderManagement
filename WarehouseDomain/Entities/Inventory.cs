namespace WarehouseDomain.Entities;

public class Inventory
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public Product Product { get; set; } = null!;
}