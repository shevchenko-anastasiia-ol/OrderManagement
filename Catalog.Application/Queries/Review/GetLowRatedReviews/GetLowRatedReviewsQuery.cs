using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetLowRatedReviews;

public class GetLowRatedReviewsQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string? ProductId { get; init; }
}