using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Product.AddProductImages;

public class AddProductImagesCommand : ICommand<Domain.Entities.Product>
{
    public string ProductId { get; init; } = default!;
    public IReadOnlyList<string> ImageUrls { get; init; } = Array.Empty<string>();
}