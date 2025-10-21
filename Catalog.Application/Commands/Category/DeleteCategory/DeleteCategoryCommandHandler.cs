using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;
using MediatR;

namespace Catalog.Application.Commands.Category.DeleteCategory;

public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryService _categoryService;
    public DeleteCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(request.CategoryId, cancellationToken);
        return Unit.Value;
    }
}