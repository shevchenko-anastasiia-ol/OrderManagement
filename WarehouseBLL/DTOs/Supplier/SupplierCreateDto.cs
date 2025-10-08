namespace WarehouseBLL.DTOs.Supplier;

public class SupplierCreateDto
{
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? ContactInfo { get; set; }
    public int CreatedBy { get; set; }
}