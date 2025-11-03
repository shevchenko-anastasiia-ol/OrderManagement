using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.SearchProductsByText;

public class SearchProductsByTextQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public string SearchTerm { get; init; } = default!;
    public int? Limit { get; init; }
}