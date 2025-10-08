namespace WarehouseBLL.DTOs.Product;

public class ProductUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public int UpdatedBy { get; set; }
}