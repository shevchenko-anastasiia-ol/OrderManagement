using Catalog.Application.Interfaces.Commands;


namespace Catalog.Application.Commands.Category;

public class CreateCategoryCommand : ICommand<Domain.Entities.Category>
{
    public string Name { get; init; } = default!;
    public string UserId { get; init; } = default!;
}