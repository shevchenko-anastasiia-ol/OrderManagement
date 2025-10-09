using Ardalis.Specification;
using WarehouseBLL.Helpers;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class ProductCountSpecification : Specification<Product>
{
    public ProductCountSpecification(ProductQueryParams queryParams)
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
    }
}