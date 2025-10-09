using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class RecentProductsSpec : Specification<Product>
{
    public RecentProductsSpec(int count = 10)
    {
        Query.Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count);
    }
}