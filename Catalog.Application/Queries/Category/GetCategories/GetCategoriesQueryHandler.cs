using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetCategories;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, PagedList<Domain.Entities.Category>>
{
    private readonly ICategoryService _categoryService;

    public GetCategoriesQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<PagedList<Domain.Entities.Category>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _categoryService.GetAllAsync(request.Parameters, cancellationToken);
    }
}