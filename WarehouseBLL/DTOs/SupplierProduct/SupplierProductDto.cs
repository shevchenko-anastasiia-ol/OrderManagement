namespace WarehouseBLL.DTOs.SupplierProduct;

public class SupplierProductDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}