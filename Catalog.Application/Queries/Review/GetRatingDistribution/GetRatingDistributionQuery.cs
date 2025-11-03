using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetRatingDistribution;

public class GetRatingDistributionQuery : IQuery<Dictionary<int, long>>
{
    public string ProductId { get; init; } = default!;
}