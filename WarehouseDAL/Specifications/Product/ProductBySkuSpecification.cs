using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class ProductBySkuSpecification : Specification<Product>, ISingleResultSpecification<Product>
{
    public ProductBySkuSpecification(string sku)
    {
        Query.Where(p => p.SKU == sku && !p.IsDeleted);
    }
}