using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Category.UpdateCategory;

public class UpdateCategoryCommand : ICommand<Domain.Entities.Category>
{
    public string CategoryId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string UserId { get; init; } = default!;
}