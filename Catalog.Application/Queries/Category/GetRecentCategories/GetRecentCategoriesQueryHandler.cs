using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetRecentCategories;

public class GetRecentCategoriesQueryHandler : IQueryHandler<GetRecentCategoriesQuery, PagedList<Domain.Entities.Category>>
{
    private readonly ICategoryService _categoryService;

    public GetRecentCategoriesQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<PagedList<Domain.Entities.Category>> Handle(GetRecentCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetRecentCategoriesAsync(request.Count, cancellationToken);
    }
}