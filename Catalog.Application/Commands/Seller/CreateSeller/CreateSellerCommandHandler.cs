using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Seller.CreateSeller;

public class CreateSellerCommandHandler : ICommandHandler<CreateSellerCommand, Domain.Entities.Seller>
{
    private readonly ISellerService _sellerService;

    public CreateSellerCommandHandler(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<Domain.Entities.Seller> Handle(CreateSellerCommand request, CancellationToken cancellationToken)
    {
        var seller = new Domain.Entities.Seller(request.Name, request.Email, request.Phone, request.UserId);
        await _sellerService.CreateAsync(seller, cancellationToken);
        return seller;
    }
}
