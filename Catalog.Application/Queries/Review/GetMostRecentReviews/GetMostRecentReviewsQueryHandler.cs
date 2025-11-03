using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetMostRecentReviews;

public class GetMostRecentReviewsQueryHandler : IQueryHandler<GetMostRecentReviewsQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetMostRecentReviewsQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetMostRecentReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetMostRecentReviewsAsync(request.Count, request.ProductId, cancellationToken);
    }
}