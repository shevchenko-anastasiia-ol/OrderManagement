using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Seller.GetSellerByPhone;

public class GetSellerByPhoneQuery : IQuery<Domain.Entities.Seller?>
{
    public string Phone { get; init; } = default!;
}