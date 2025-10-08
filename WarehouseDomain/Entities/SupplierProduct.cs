namespace WarehouseDomain.Entities;

public class SupplierProduct
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? RowVersion { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public Product Product { get; set; } = null!;
}