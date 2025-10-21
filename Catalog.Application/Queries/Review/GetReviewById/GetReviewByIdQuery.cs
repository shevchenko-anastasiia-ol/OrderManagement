using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetReviewById;

public class GetReviewByIdQuery : IQuery<Domain.Entities.Review>
{
    public string ReviewId { get; init; } = default!;
}