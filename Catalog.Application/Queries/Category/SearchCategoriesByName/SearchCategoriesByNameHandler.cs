using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Category.SearchCategoriesByName;

public class SearchCategoriesByNameQueryHandler : IQueryHandler<SearchCategoriesByNameQuery, IEnumerable<Domain.Entities.Category>>
{
    private readonly ICategoryService _categoryService;

    public SearchCategoriesByNameQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IEnumerable<Domain.Entities.Category>> Handle(SearchCategoriesByNameQuery request, CancellationToken cancellationToken)
    {
        return await _categoryService.SearchByNameAsync(request.Name, cancellationToken);
    }
}