using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Review.DeleteAllProductReviews;

public class DeleteAllProductReviewsCommand : ICommand<long>
{
    public string ProductId { get; init; } = default!;
}