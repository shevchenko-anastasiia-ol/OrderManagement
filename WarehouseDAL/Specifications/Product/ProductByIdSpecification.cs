using Ardalis.Specification;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Specifications;

public class ProductByIdSpecification : Specification<Product>, ISingleResultSpecification<Product>
{
    public ProductByIdSpecification(int id, bool includeInventories = false, bool includeSuppliers = false)
    {
        Query.Where(p => p.Id == id && !p.IsDeleted);

        if (includeInventories)
        {
            Query.Include(p => p.Inventories)
                .ThenInclude(i => i.Warehouse);
        }

        if (includeSuppliers)
        {
            Query.Include(p => p.SupplierProducts)
                .ThenInclude(sp => sp.Supplier);
        }
    }
}