using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Seller.UpdateSeller;

public class UpdateSellerCommandHandler : ICommandHandler<UpdateSellerCommand, Domain.Entities.Seller>
{
    private readonly ISellerService _sellerService;

    public UpdateSellerCommandHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Domain.Entities.Seller> Handle(UpdateSellerCommand request, CancellationToken cancellationToken)
    {
        var seller = await _sellerService.GetByIdAsync(request.SellerId, cancellationToken);
        seller.Update(request.Name, request.Email, request.Phone, request.UserId);
        await _sellerService.UpdateAsync(seller, cancellationToken);
        return seller;
    }
}