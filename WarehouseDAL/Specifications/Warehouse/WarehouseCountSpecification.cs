using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Warehouse;

public class WarehouseCountSpecification : Specification<WarehouseDomain.Entities.Warehouse>
{
    public WarehouseCountSpecification(WarehouseQueryParams queryParams)
    {
        Query.Where(w => !w.IsDeleted);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            Query.Where(w => w.Name.Contains(queryParams.SearchTerm));

        if (queryParams.MinCapacity.HasValue)
            Query.Where(w => w.Capacity >= queryParams.MinCapacity.Value);

        if (queryParams.MaxCapacity.HasValue)
            Query.Where(w => w.Capacity <= queryParams.MaxCapacity.Value);
    }
}