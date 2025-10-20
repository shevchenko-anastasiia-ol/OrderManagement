using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces.Services;

public interface ICategoryService
{
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsCategoryInUseAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default);
    Task<long> GetProductCountAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<bool> AllExistAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetNonExistingIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetEmptyCategoriesAsync(CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetAllSortedByNameAsync(bool ascending = true, CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetRecentCategoriesAsync(int count, CancellationToken cancellationToken = default);
}