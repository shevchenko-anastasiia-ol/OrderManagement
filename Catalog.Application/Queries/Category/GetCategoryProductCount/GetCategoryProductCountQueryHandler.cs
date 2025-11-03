using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetCategoryProductCount;

public class GetCategoryProductCountQueryHandler : IQueryHandler<GetCategoryProductCountQuery, long>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryProductCountQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<long> Handle(GetCategoryProductCountQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetProductCountAsync(request.CategoryId, cancellationToken);
    }
}