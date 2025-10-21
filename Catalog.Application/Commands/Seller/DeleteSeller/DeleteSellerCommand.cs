using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Seller.DeleteSeller;

public class DeleteSellerCommand : ICommand
{
    public string SellerId { get; init; } = default!;
}