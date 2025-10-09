using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Warehouse;

public class WarehouseSpecification : Specification<WarehouseDomain.Entities.Warehouse>
{
    public WarehouseSpecification(WarehouseQueryParams queryParams)
    {
        Query.Where(w => !w.IsDeleted);

        // Фільтри
        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            Query.Where(w => w.Name.Contains(queryParams.SearchTerm));

        if (queryParams.MinCapacity.HasValue)
            Query.Where(w => w.Capacity >= queryParams.MinCapacity.Value);

        if (queryParams.MaxCapacity.HasValue)
            Query.Where(w => w.Capacity <= queryParams.MaxCapacity.Value);

        // Сортування
        ApplySorting(queryParams.SortBy, queryParams.SortDirection);

        // Пагінація
        Query.Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize);
    }

    private void ApplySorting(string? sortBy, string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            Query.OrderBy(w => w.Name);
            return;
        }

        var isDesc = sortDirection.ToLower() == "desc";

        switch (sortBy.ToLower())
        {
            case "name":
                if (isDesc) Query.OrderByDescending(w => w.Name);
                else Query.OrderBy(w => w.Name);
                break;
            case "capacity":
                if (isDesc) Query.OrderByDescending(w => w.Capacity);
                else Query.OrderBy(w => w.Capacity);
                break;
            default:
                Query.OrderBy(w => w.Name);
                break;
        }
    }
}