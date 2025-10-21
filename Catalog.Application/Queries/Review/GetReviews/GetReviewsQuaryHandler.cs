using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetReviews;

public class GetReviewsQuaryHandler : IQueryHandler<GetReviewsQuery, PagedList<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetReviewsQuaryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<PagedList<Domain.Entities.Review>> Handle(GetReviewsQuery request,
        CancellationToken cancellationToken)
    {
        return await _reviewService.GetAllAsync(request.Parameters, cancellationToken);
    }
}