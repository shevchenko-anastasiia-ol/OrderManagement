using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductsByCategory;

public class GetProductsByCategoryQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public string CategoryId { get; init; } = default!;
}