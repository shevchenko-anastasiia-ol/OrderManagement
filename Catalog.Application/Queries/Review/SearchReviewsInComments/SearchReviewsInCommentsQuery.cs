using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.SearchReviewsInComments;

public class SearchReviewsInCommentsQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string SearchTerm { get; init; } = default!;
}