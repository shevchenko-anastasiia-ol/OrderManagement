using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.GetSellerByEmail;

public class GetSellerByEmailQuery : IQuery<Domain.Entities.Seller?>
{
    public string Email { get; init; } = default!;
}