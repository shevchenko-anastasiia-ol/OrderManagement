namespace WarehouseDomain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
}