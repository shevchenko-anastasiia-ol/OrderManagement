using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Commands.Product.CreateProduct;

public class CreateProductCommand : ICommand<Domain.Entities.Product>
{
    public string Name { get; init; } = default!;
    public Money Price { get; init; } = default!;
    public string SellerId { get; init; } = default!;
    public IReadOnlyList<string> CategoryIds { get; init; } = Array.Empty<string>();
    public string? Description { get; init; }
    public IReadOnlyDictionary<string, string>? Specifications { get; init; }
    public IReadOnlyList<string>? ImageUrls { get; init; }
    public string UserId { get; init; } = default!;
}