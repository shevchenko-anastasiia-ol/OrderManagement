using Ardalis.Specification;
using WarehouseBLL.Helpers;

namespace WarehouseBLL.Specifications.Supplier;

public class SupplierSpecification : Specification<WarehouseDomain.Entities.Supplier>
{
    public SupplierSpecification(SupplierQueryParams queryParams)
    {
        Query.Where(s => !s.IsDeleted);

        // Фільтри
        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            Query.Where(s => s.Name.Contains(queryParams.SearchTerm) || 
                             s.ContactInfo.Contains(queryParams.SearchTerm));

        if (!string.IsNullOrWhiteSpace(queryParams.Country))
            Query.Where(s => s.Country == queryParams.Country);

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
            Query.OrderBy(s => s.Name);
            return;
        }

        var isDesc = sortDirection.ToLower() == "desc";

        switch (sortBy.ToLower())
        {
            case "name":
                if (isDesc) Query.OrderByDescending(s => s.Name);
                else Query.OrderBy(s => s.Name);
                break;
            case "country":
                if (isDesc) Query.OrderByDescending(s => s.Country);
                else Query.OrderBy(s => s.Country);
                break;
            case "contactInfo":
                if (isDesc) Query.OrderByDescending(s => s.ContactInfo);
                else Query.OrderBy(s => s.ContactInfo);
                break;
            default:
                Query.OrderBy(s => s.Name);
                break;
        }
    }
}