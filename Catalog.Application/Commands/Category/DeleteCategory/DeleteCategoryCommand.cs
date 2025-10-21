using Catalog.Application.Interfaces.Commands;

namespace Catalog.Application.Commands.Category.DeleteCategory;

public class DeleteCategoryCommand : ICommand
{
    public string CategoryId { get; init; } = default!;

}