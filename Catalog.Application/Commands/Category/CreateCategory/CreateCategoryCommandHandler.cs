using Catalog.Application.Interfaces.Commands;
using Catalog.Domain.Interfaces.Services;

namespace Catalog.Application.Commands.Category;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Domain.Entities.Category>
{
    private readonly ICategoryService _categoryService;
    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    public async Task<Domain.Entities.Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Domain.Entities.Category(request.Name, request.UserId);
        await _categoryService.CreateAsync(category, cancellationToken);
        return category;
    }
}