namespace WarehouseDomain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? ContactInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
}