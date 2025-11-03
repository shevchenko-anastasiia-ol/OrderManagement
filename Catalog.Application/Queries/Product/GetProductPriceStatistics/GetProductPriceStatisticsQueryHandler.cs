using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Product.GetProductPriceStatistics;

public class GetProductPriceStatisticsQueryHandler : IQueryHandler<GetProductPriceStatisticsQuery, PriceStatistics>
{
    private readonly IProductService _productService;

    public GetProductPriceStatisticsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<PriceStatistics> Handle(GetProductPriceStatisticsQuery request, CancellationToken cancellationToken)
    {
        var minPrice = await _productService.GetMinPriceAsync(request.CategoryId, request.Currency, cancellationToken);
        var maxPrice = await _productService.GetMaxPriceAsync(request.CategoryId, request.Currency, cancellationToken);
        var avgPrice = await _productService.GetAveragePriceAsync(request.CategoryId, request.Currency, cancellationToken);

        return new PriceStatistics
        {
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            AveragePrice = avgPrice
        };
    }
}