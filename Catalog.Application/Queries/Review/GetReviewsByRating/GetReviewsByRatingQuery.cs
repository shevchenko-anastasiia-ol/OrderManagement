using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetReviewsByRating;

public class GetReviewsByRatingQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public int Rating { get; init; }
}