using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.IsCategoryInUse;

public class IsCategoryInUseQueryHandler : IQueryHandler<IsCategoryInUseQuery, bool>
{
    private readonly ICategoryService _categoryService;

    public IsCategoryInUseQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<bool> Handle(IsCategoryInUseQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.IsCategoryInUseAsync(request.CategoryId, cancellationToken);
    }
}