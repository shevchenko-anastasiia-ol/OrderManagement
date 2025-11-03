using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetCategoryByName;

public class GetCategoryByNameQueryHandler : IQueryHandler<GetCategoryByNameQuery, Domain.Entities.Category?>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByNameQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Domain.Entities.Category?> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.GetByNameAsync(request.Name, cancellationToken);
    }
}