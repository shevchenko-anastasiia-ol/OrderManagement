using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;

namespace Catalog.Application.Queries.Product.GetProductById;

public class GetProductByIdQuery : IQuery<Domain.Entities.Product>
{
    public string ProductId { get; init; } = default!;
}