using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Inventory;

public class InventorySpecification : Specification<WarehouseDomain.Entities.Inventory>
{
    public InventorySpecification(InventoryQueryParams queryParams)
    {
        Query.AsNoTracking();

        // Фільтри
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

        // Includes
        Query.Include(i => i.Warehouse)
             .Include(i => i.Product);

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
            Query.OrderBy(i => i.Id);
            return;
        }

        var isDesc = sortDirection.ToLower() == "desc";

        switch (sortBy.ToLower())
        {
            case "quantity":
                if (isDesc) Query.OrderByDescending(i => i.Quantity);
                else Query.OrderBy(i => i.Quantity);
                break;
            case "warehouseid":
                if (isDesc) Query.OrderByDescending(i => i.WarehouseId);
                else Query.OrderBy(i => i.WarehouseId);
                break;
            case "productid":
                if (isDesc) Query.OrderByDescending(i => i.ProductId);
                else Query.OrderBy(i => i.ProductId);
                break;
            default:
                Query.OrderBy(i => i.Id);
                break;
        }
    }
}