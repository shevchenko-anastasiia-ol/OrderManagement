using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Category.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Domain.Entities.Category>
{
    private readonly ICategoryService _categoryService;
    public UpdateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    public async Task<Domain.Entities.Category> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(request.CategoryId, cancellationToken);
        category.Update(request.Name, request.UserId);
        await _categoryService.UpdateAsync(category, cancellationToken);
        return category;
    }
}