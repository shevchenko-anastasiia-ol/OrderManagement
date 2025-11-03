using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.SearchReviewsInComments;

public class SearchReviewsInCommentsQueryHandler : IQueryHandler<SearchReviewsInCommentsQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public SearchReviewsInCommentsQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(SearchReviewsInCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.SearchInCommentsAsync(request.SearchTerm, cancellationToken);
    }
}