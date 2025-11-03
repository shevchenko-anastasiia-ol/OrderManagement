using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductsSortedByName;

public class GetProductsSortedByNameQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public bool Ascending { get; init; } = true;
    public int? Limit { get; init; }
}