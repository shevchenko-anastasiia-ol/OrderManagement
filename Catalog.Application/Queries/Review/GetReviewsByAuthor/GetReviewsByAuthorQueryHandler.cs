using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Queries.Review.GetReviewsByAuthor;

public class GetReviewsByAuthorQueryHandler : IQueryHandler<GetReviewsByAuthorQuery, IEnumerable<Domain.Entities.Review>>
{
    private readonly IReviewService _reviewService;

    public GetReviewsByAuthorQueryHandler(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IEnumerable<Domain.Entities.Review>> Handle(GetReviewsByAuthorQuery request, CancellationToken cancellationToken)
    {
        return await _reviewService.GetByAuthorAsync(request.Author, cancellationToken);
    }
}