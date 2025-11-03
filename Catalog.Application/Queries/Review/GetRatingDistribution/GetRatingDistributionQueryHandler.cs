using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetRatingDistribution;

public class GetRatingDistributionQueryHandler : IQueryHandler<GetRatingDistributionQuery, Dictionary<int, long>>
{
    private readonly IReviewService _reviewService;

    public GetRatingDistributionQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<Dictionary<int, long>> Handle(GetRatingDistributionQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetRatingDistributionAsync(request.ProductId, cancellationToken);
    }
}