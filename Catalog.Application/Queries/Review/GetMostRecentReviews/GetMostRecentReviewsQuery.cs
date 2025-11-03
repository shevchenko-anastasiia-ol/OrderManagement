using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetMostRecentReviews;

public class GetMostRecentReviewsQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public int Count { get; init; } = 10;
    public string? ProductId { get; init; }
}