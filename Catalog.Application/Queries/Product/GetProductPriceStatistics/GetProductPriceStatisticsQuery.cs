using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductPriceStatistics;

public class GetProductPriceStatisticsQuery : IQuery<PriceStatistics>
{
    public string? CategoryId { get; init; }
    public string? Currency { get; init; }
}