using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductsSortedByPrice;

public class GetProductsSortedByPriceQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public bool Ascending { get; init; } = true;
    public int? Limit { get; init; }
}