using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetReviewsByProduct;

public class GetReviewsByProductQuery : IQuery<IEnumerable<Domain.Entities.Review>>
{
    public string ProductId { get; init; } = default!;
}