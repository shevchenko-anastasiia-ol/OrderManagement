using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Inventory;

public class InventoryCountSpecification : Specification<WarehouseDomain.Entities.Inventory>
{
    public InventoryCountSpecification(InventoryQueryParams queryParams)
    {
        if (queryParams.WarehouseId.HasValue)
            Query.Where(i => i.WarehouseId == queryParams.WarehouseId.Value);

        if (queryParams.ProductId.HasValue)
            Query.Where(i => i.ProductId == queryParams.ProductId.Value);

        if (queryParams.MinQuantity.HasValue)
            Query.Where(i => i.Quantity >= queryParams.MinQuantity.Value);

        if (queryParams.MaxQuantity.HasValue)
            Query.Where(i => i.Quantity <= queryParams.MaxQuantity.Value);

        if (queryParams.LowStock.HasValue && queryParams.LowStock.Value)
        {
            var threshold = queryParams.LowStockThreshold ?? 10;
            Query.Where(i => i.Quantity <= threshold);
        }
    }
}