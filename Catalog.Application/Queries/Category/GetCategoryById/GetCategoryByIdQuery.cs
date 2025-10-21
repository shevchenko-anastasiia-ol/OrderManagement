using Catalog.Application.Interfaces.Queries;

namespace Catalog.Application.Queries.Category.GetCategoryById;

public class GetCategoryByIdQuery :  IQuery<Domain.Entities.Category>
{
    public string CategoryId { get; init; } = default!;
}