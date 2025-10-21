using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.GetSellerById;

public class GetSellerByIdQuery : IQuery<Domain.Entities.Seller>
{
    public string SellerId { get; init; } = default!;
}