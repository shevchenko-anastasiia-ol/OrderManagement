using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetHighRatedReviews;

public class GetHighRatedReviewsQueryHandler : IQueryHandler<GetHighRatedReviewsQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetHighRatedReviewsQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetHighRatedReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetHighRatedReviewsAsync(request.ProductId, cancellationToken);
    }
}