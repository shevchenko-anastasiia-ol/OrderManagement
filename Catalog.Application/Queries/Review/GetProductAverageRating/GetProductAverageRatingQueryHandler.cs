using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetProductAverageRating;

public class GetProductAverageRatingQueryHandler : IQueryHandler<GetProductAverageRatingQuery, double>
{
    private readonly IReviewService _reviewService;

    public GetProductAverageRatingQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<double> Handle(GetProductAverageRatingQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetAverageRatingAsync(request.ProductId, cancellationToken);
    }
}