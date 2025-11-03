using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductCountByCategory;

public class GetProductCountByCategoryQueryHandler : IQueryHandler<GetProductCountByCategoryQuery, Dictionary<string, long>>
{
    private readonly IProductService _productService;

    public GetProductCountByCategoryQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Dictionary<string, long>> Handle(GetProductCountByCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductCountByCategoryAsync(cancellationToken);
    }
}