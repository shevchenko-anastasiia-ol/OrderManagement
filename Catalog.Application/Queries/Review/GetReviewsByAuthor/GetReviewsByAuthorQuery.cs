using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetReviewsByAuthor;

public class GetReviewsByAuthorQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string Author { get; init; } = default!;
}