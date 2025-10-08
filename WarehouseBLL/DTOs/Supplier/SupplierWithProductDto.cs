using WarehouseBLL.DTOs.Product;

namespace WarehouseBLL.DTOs.Supplier;

public class SupplierWithProductDto :  SupplierDto
{
    public List<ProductDto> Products { get; set; } = new();
}