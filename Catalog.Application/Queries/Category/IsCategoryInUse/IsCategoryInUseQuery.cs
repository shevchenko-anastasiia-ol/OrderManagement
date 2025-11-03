using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Category.IsCategoryInUse;

public class IsCategoryInUseQuery : IQuery<bool>
{
    public string CategoryId { get; init; } = default!;
}