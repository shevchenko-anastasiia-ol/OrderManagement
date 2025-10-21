using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.GetCategoryById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Domain.Entities.Category>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Domain.Entities.Category> Handle(GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _categoryService.GetByIdAsync(request.CategoryId, cancellationToken);
    }
}