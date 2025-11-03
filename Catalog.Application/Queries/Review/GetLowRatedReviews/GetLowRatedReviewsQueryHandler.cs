using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetLowRatedReviews;

public class GetLowRatedReviewsQueryHandler : IQueryHandler<GetLowRatedReviewsQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetLowRatedReviewsQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetLowRatedReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetLowRatedReviewsAsync(request.ProductId, cancellationToken);
    }
}