using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Category.GetCategoryByName;

public class GetCategoryByNameQuery : IQuery<Domain.Entities.Category?>
{
    public string Name { get; init; } = default!;
}