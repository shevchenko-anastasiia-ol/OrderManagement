using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetReviewsByProduct;

public class GetReviewsByProductQueryHandler : IQueryHandler<GetReviewsByProductQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetReviewsByProductQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetByProductAsync(request.ProductId, cancellationToken);
    }
}