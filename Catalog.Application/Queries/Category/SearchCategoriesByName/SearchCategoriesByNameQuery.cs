using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Category.SearchCategoriesByName;

public class SearchCategoriesByNameQuery : IQuery<IEnumerable<Domain.Entities.Category>>
{
    public string Name { get; init; } = default!;
}