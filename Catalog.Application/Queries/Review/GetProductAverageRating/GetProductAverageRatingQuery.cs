using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Review.GetProductAverageRating;

public class GetProductAverageRatingQuery : IQuery<double>
{
    public string ProductId { get; init; } = default!;
}