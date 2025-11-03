using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;

namespace Catalog.Application.Queries.Category.GetEmptyCategories;

public class GetEmptyCategoriesQuery : IQuery<PagedList<Domain.Entities.Category>>
{
}