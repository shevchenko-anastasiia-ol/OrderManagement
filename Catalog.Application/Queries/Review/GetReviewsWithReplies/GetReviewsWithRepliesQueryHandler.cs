using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetReviewsWithReplies;

public class GetReviewsWithRepliesQueryHandler : IQueryHandler<GetReviewsWithRepliesQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetReviewsWithRepliesQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetReviewsWithRepliesQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetReviewsWithRepliesAsync(request.ProductId, cancellationToken);
    }
}