namespace WarehouseBLL.DTOs.Product;

public class ProductCreateDto
{
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public int CreatedBy { get; set; }
}