using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetReviewsWithReplies;

public class GetReviewsWithRepliesQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string? ProductId { get; init; }
}