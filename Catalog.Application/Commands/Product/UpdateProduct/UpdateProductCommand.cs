using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Commands.Product.UpdateProduct;

public class UpdateProductCommand : ICommand<Domain.Entities.Product>
{
    public string ProductId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Money Price { get; init; } = default!;
    public IReadOnlyList<string>? CategoryIds { get; init; }
    public string? Description { get; init; }
    public IReadOnlyDictionary<string, string>? Specifications { get; init; }
    public IReadOnlyList<string>? ImageUrls { get; init; }
    public string UserId { get; init; } = default!;
    public int? ExpectedVersion { get; init; }
}