using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Seller.DeleteSeller;

public class DeleteSellerCommandHandler : ICommandHandler<DeleteSellerCommand>
{
    private readonly ISellerService _sellerService;

    public DeleteSellerCommandHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Unit> Handle(DeleteSellerCommand request, CancellationToken cancellationToken)
    {
        await _sellerService.DeleteAsync(request.SellerId, cancellationToken);
        return Unit.Value;
    }
}