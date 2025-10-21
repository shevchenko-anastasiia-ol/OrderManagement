using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Domain.Interfaces.Services;

public interface IProductService
    {
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedList<Product>> GetAllAsync(ProductParameters parameters, CancellationToken cancellationToken = default);
        Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, bool deleteReviews, CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> SearchByTextAsync(string searchTerm, int? limit = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<PagedList<Product>> GetByCategoryCursorAsync(string categoryId, string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByMultipleCategoriesAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
        Task<(PagedList<Product> Items, long TotalCount)> GetPagedByCategoryAsync(string categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBySellerAsync(string sellerId, CancellationToken cancellationToken = default);
        Task<(PagedList<Product> Items, long TotalCount)> GetPagedBySellerAsync(string sellerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBelowPriceAsync(decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAbovePriceAsync(decimal minPrice, string? currency = null, CancellationToken cancellationToken = default);
        Task<Dictionary<string, long>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<string, long>> GetProductCountBySellerAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetMinPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default);
        Task<decimal> GetMaxPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default);
        Task<bool> AddCategoryToProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default);
        Task<bool> RemoveCategoryFromProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductCategoriesAsync(string productId, IEnumerable<string> categoryIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetProductIdsByCategoryAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<int> UpdatePriceAsync(Product priceUpdates, CancellationToken cancellationToken = default);
        Task<int> UpdateSellerAsync(string oldSellerId, string newSellerId, CancellationToken cancellationToken = default);
        Task<int> RemoveCategoryFromAllProductsAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<int> ReplaceCategoryInAllProductsAsync(string oldCategoryId, string newCategoryId, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductDetailAsync(string productId, ProductDetail productDetail, CancellationToken cancellationToken = default);
        Task<bool> AddImageToProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default);
        Task<bool> RemoveImageFromProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default);
        Task<bool> AddSpecificationAsync(string productId, string key, string value, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLatestProductsAsync(int count, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsSortedByPriceAsync(bool ascending = true, int? limit = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsSortedByNameAsync(bool ascending = true, int? limit = null, CancellationToken cancellationToken = default);
    }