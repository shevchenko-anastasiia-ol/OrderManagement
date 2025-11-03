using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Category.GetCategoryProductCount;

public class GetCategoryProductCountQuery : IQuery<long>
{
    public string CategoryId { get; init; } = default!;
}