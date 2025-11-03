using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Product.GetProductsBySeller;

public class GetProductsBySellerQuery : IQuery<IEnumerable<Domain.Entities.Product>>
{
    public string SellerId { get; init; } = default!;
}