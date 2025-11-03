using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsByCategoryQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetByCategoryAsync(request.CategoryId, cancellationToken);
    }
}