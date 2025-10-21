using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Interfaces.Repositories;

namespace Catalog.Application.Queries.Review.GetReviewById;

public class GetReviewByIdQueryHandler : IQueryHandler<GetReviewByIdQuery, Domain.Entities.Review>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Domain.Entities.Review> Handle(GetReviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _reviewRepository.GetByIdAsync(request.ReviewId,  cancellationToken);
    }
}