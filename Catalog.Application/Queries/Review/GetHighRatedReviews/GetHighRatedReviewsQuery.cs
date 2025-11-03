using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetHighRatedReviews;

public class GetHighRatedReviewsQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string? ProductId { get; init; }
}