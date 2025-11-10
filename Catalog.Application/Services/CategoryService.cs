using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Exceptions;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using FluentValidation.Results;

namespace Catalog.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (string.IsNullOrWhiteSpace(category.Name))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(category.Name), "Category name cannot be empty") });

        if (await _categoryRepository.ExistsByNameAsync(category.Name, cancellationToken: cancellationToken))
            throw new ConflictException($"Category with name '{category.Name}' already exists");

        return await _categoryRepository.CreateAsync(category, cancellationToken);
    }

    public async Task<Category> CreateCategoryAsync(string name, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(name), "Category name cannot be empty") });

        if (string.IsNullOrWhiteSpace(userId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(userId), "User ID cannot be empty") });

        if (await _categoryRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
            throw new ConflictException($"Category with name '{name}' already exists");

        var category = new Category(name, userId);
        return await _categoryRepository.CreateAsync(category, cancellationToken);
    }

    public async Task<Category?> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (string.IsNullOrWhiteSpace(category.Id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(category.Id), "Category ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(category.Name))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(category.Name), "Category name cannot be empty") });

        var existing = await _categoryRepository.GetByIdAsync(category.Id, cancellationToken);
        if (existing == null)
            throw new NotFoundException($"Category with ID '{category.Id}' not found");

        if (await _categoryRepository.ExistsByNameAsync(category.Name, category.Id, cancellationToken))
            throw new ConflictException($"Category with name '{category.Name}' already exists");

        return await _categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task<Category> UpdateCategoryAsync(string id, string name, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(id), "Category ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(name))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(name), "Category name cannot be empty") });

        if (string.IsNullOrWhiteSpace(userId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(userId), "User ID cannot be empty") });

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new NotFoundException($"Category with ID '{id}' not found");

        if (await _categoryRepository.ExistsByNameAsync(name, id, cancellationToken))
            throw new ConflictException($"Category with name '{name}' already exists");

        category.Update(name, userId);
        return await _categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(id), "Category ID cannot be empty") });

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return false;

        if (await _categoryRepository.IsCategoryInUseAsync(id, cancellationToken))
            throw new ConflictException($"Cannot delete category '{category.Name}' because it is in use by products");

        return await _categoryRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task DeleteCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(id), "Category ID cannot be empty") });

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new NotFoundException($"Category with ID '{id}' not found");

        if (await _categoryRepository.IsCategoryInUseAsync(id, cancellationToken))
            throw new ConflictException($"Cannot delete category '{category.Name}' because it is in use by products");

        var deleted = await _categoryRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            throw new ConcurrencyException(id, expectedVersion: 1, actualVersion: 0);
    }

    public async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _categoryRepository.GetByNameAsync(name, cancellationToken);
    }

    public async Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _categoryRepository.GetByNameAsync(name, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<PagedList<Category>> GetAllAsync(CategoryParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        return await _categoryRepository.GetAllSortedByNameAsync(
            parameters.PageNumber,
            parameters.PageSize,
            true,
            parameters.OrderBy,
            cancellationToken);
    }

    public async Task<PagedList<Category>> GetCategoriesPagedAsync(
        int pageNumber, 
        int pageSize, 
        bool ascending = true, 
        string? orderBy = null, 
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _categoryRepository.GetAllSortedByNameAsync(
            pageNumber, 
            pageSize, 
            ascending, 
            orderBy, 
            cancellationToken);
    }

    public async Task<PagedList<Category>> GetAllSortedByNameAsync(
        int pageNumber,
        int pageSize,
        bool ascending = true,
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _categoryRepository.GetAllSortedByNameAsync(
            pageNumber,
            pageSize,
            ascending,
            orderBy,
            cancellationToken);
    }

    public async Task<IEnumerable<Category>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Array.Empty<Category>();

        return await _categoryRepository.SearchByNameAsync(name, cancellationToken);
    }

    public async Task<IEnumerable<Category>> SearchCategoriesByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Array.Empty<Category>();

        return await _categoryRepository.SearchByNameAsync(name, cancellationToken);
    }

    public async Task<bool> IsCategoryInUseAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return false;

        return await _categoryRepository.IsCategoryInUseAsync(categoryId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return await _categoryRepository.ExistsByNameAsync(name, excludeId, cancellationToken);
    }

    public async Task<long> GetProductCountAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return 0;

        return await _categoryRepository.GetProductCountAsync(categoryId, cancellationToken);
    }

    public async Task<long> GetProductCountByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return 0;

        return await _categoryRepository.GetProductCountAsync(categoryId, cancellationToken);
    }

    public async Task<PagedList<Category>> GetEmptyCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetEmptyCategoriesAsync(cancellationToken);
        var list = categories.ToList();
        return new PagedList<Category>(list, list.Count, 1, list.Count);
    }

    public async Task<PagedList<Category>> GetRecentCategoriesAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        var categories = await _categoryRepository.GetRecentCategoriesAsync(count, cancellationToken);
        var list = categories.ToList();
        return new PagedList<Category>(list, list.Count, 1, count);
    }

    public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
    {
        if (categoryIds == null || !categoryIds.Any())
            return Array.Empty<Category>();

        return await _categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
    }

    public async Task<bool> AllExistAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
    {
        if (categoryIds == null || !categoryIds.Any())
            return false;

        return await _categoryRepository.AllExistAsync(categoryIds, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetNonExistingIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
    {
        if (categoryIds == null || !categoryIds.Any())
            return Array.Empty<string>();

        return await _categoryRepository.GetNonExistingIdsAsync(categoryIds, cancellationToken);
    }
}