using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Review.AddReviewReply;

public class AddReviewReplyCommand : ICommand<Domain.Entities.Review>
{
    public string ReviewId { get; init; } = default!;
    public string Author { get; init; } = default!;
    public string Text { get; init; } = default!;
}