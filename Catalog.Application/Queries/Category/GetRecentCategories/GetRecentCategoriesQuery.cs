using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;

namespace Catalog.Application.Queries.Category.GetRecentCategories;

public class GetRecentCategoriesQuery : IQuery<PagedList<Domain.Entities.Category>>
{
    public int Count { get; init; } = 10;
}