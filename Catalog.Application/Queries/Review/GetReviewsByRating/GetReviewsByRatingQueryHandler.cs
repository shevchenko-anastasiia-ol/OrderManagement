using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetReviewsByRating;

public class GetReviewsByRatingQueryHandler : IQueryHandler<GetReviewsByRatingQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetReviewsByRatingQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetReviewsByRatingQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetByRatingAsync(request.Rating, cancellationToken);
    }
}