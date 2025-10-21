using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Application.Queries.Category.GetCategories;

public class GetCategoriesQuery : IQuery<PagedList<Domain.Entities.Category>>
{
    public CategoryParameters Parameters { get; init; } = default!;
}