using WarehouseBLL.DTOs.Supplier;

namespace WarehouseBLL.DTOs.Product;

public class ProductWithSuppliersDto : ProductDto
{
    public List<SupplierDto> Suppliers { get; set; } = new();
}