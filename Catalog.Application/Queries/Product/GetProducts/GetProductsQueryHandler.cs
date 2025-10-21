using Catalog.Application.Interfaces.Queries;
using Catalog.Application.Queries.Product.GetProductById;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProducts;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedList<Domain.Entities.Product>>
{
    private readonly IProductService _productService;

    public GetProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<PagedList<Domain.Entities.Product>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _productService.GetAllAsync(request.Parameters, cancellationToken);
    }
}