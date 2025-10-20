using Catalog.Domain.Common;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsCategoryInUseAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default);
    Task<long> GetProductCountAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<bool> AllExistAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetNonExistingIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetEmptyCategoriesAsync(CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetAllSortedByNameAsync(int pageNumber, int pageSize, bool ascending = true, string? orderBy = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetRecentCategoriesAsync(int count, CancellationToken cancellationToken = default);
    Task<PagedList<Category>> GetByCursorAsync(string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default);
}