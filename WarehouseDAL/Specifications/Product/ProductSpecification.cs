using Ardalis.Specification;
using WarehouseBLL.Helpers;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class ProductSpecification : Specification<Product>
{
    public ProductSpecification(ProductQueryParams queryParams)
    {
        Query.Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            Query.Where(p => p.Name.Contains(queryParams.SearchTerm) || p.SKU.Contains(queryParams.SearchTerm));

        if (!string.IsNullOrWhiteSpace(queryParams.Sku))
            Query.Where(p => p.SKU == queryParams.Sku);

        if (queryParams.MinPrice.HasValue)
            Query.Where(p => p.Price >= queryParams.MinPrice.Value);

        if (queryParams.MaxPrice.HasValue)
            Query.Where(p => p.Price <= queryParams.MaxPrice.Value);

        ApplySorting(queryParams.SortBy, queryParams.SortDirection);
        Query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize);
    }

    private void ApplySorting(string? sortBy, string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            Query.OrderBy(p => p.Name);
            return;
        }

        var isDesc = sortDirection.ToLower() == "desc";

        switch (sortBy.ToLower())
        {
            case "name":
                if (isDesc) Query.OrderByDescending(p => p.Name);
                else Query.OrderBy(p => p.Name);
                break;
            case "sku":
                if (isDesc) Query.OrderByDescending(p => p.SKU);
                else Query.OrderBy(p => p.SKU);
                break;
            case "price":
                if (isDesc) Query.OrderByDescending(p => p.Price);
                else Query.OrderBy(p => p.Price);
                break;
            case "createdat":
                if (isDesc) Query.OrderByDescending(p => p.CreatedAt);
                else Query.OrderBy(p => p.CreatedAt);
                break;
            default:
                Query.OrderBy(p => p.Name);
                break;
        }
    }
}