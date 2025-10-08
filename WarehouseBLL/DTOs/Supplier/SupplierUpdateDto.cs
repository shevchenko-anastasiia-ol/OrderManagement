namespace WarehouseBLL.DTOs.Supplier;

public class SupplierUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? ContactInfo { get; set; }
    public int UpdatedBy { get; set; }
}