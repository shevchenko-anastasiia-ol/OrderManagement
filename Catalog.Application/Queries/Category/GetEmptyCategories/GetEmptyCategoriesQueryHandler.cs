using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetEmptyCategories;

public class GetEmptyCategoriesQueryHandler : IQueryHandler<GetEmptyCategoriesQuery, PagedList<Domain.Entities.Category>>
{
    private readonly ICategoryService _categoryService;

    public GetEmptyCategoriesQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<PagedList<Domain.Entities.Category>> Handle(GetEmptyCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetEmptyCategoriesAsync(cancellationToken);
    }
}