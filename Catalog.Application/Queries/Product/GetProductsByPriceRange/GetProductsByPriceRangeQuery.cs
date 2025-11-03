using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductsByPriceRange;

public class GetProductsByPriceRangeQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public decimal MinPrice { get; init; }
    public decimal MaxPrice { get; init; }
    public string? Currency { get; init; }
}