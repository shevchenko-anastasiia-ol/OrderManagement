using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class LowStockProductSpecification : Specification<Product>
{
    public LowStockProductSpecification(int threshold = 10)
    {
        Query.Where(p => !p.IsDeleted)
            .Include(p => p.Inventories)
            .ThenInclude(i => i.Warehouse)
            .Where(p => p.Inventories.Sum(i => i.Quantity) <= threshold)
            .OrderBy(p => p.Inventories.Sum(i => i.Quantity));
    }
}