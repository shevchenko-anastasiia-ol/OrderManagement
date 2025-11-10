using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.Exceptions;
using FluentValidation.Results;

namespace Catalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IReviewRepository _reviewRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ISellerRepository sellerRepository,
        IReviewRepository reviewRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _sellerRepository = sellerRepository ?? throw new ArgumentNullException(nameof(sellerRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (!await _sellerRepository.ExistsAsync(product.SellerId, cancellationToken))
            throw new NotFoundException($"Seller with ID '{product.SellerId}' does not exist");

        if (product.Categories != null && product.Categories.Any())
        {
            var categoriesExist = await _categoryRepository.AllExistAsync(product.Categories, cancellationToken);
            if (!categoriesExist)
            {
                var nonExistingIds = await _categoryRepository.GetNonExistingIdsAsync(product.Categories, cancellationToken);
                throw new NotFoundException($"Categories do not exist: {string.Join(", ", nonExistingIds)}");
            }
        }

        return await _productRepository.CreateAsync(product, cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedList<Product>> GetAllAsync(ProductParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var (items, totalCount) = await _productRepository.GetPagedAsync(
            parameters.PageNumber,
            parameters.PageSize,
            filter: null,
            orderBy: p => p.CreatedAt,
            ascending: false,
            cancellationToken);

        return new PagedList<Product>(items.ToList(), (int)totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Product?> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var existing = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
        if (existing == null)
            throw new NotFoundException($"Product with ID '{product.Id}' not found");

        if (!await _sellerRepository.ExistsAsync(product.SellerId, cancellationToken))
            throw new NotFoundException($"Seller with ID '{product.SellerId}' does not exist");

        if (product.Categories != null && product.Categories.Any())
        {
            var categoriesExist = await _categoryRepository.AllExistAsync(product.Categories, cancellationToken);
            if (!categoriesExist)
            {
                var nonExistingIds = await _categoryRepository.GetNonExistingIdsAsync(product.Categories, cancellationToken);
                throw new NotFoundException($"Categories do not exist: {string.Join(", ", nonExistingIds)}");
            }
        }

        return await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, bool deleteReviews, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(id), "Product ID cannot be empty") });

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        if (deleteReviews)
        {
            await _reviewRepository.DeleteAllProductReviewsAsync(id, cancellationToken);
        }
        else
        {
            var reviewCount = await _reviewRepository.GetProductReviewCountAsync(id, cancellationToken);
            if (reviewCount > 0)
                throw new ConflictException($"Cannot delete product. It has {reviewCount} reviews. Set deleteReviews to true to delete reviews as well.");
        }

        return await _productRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchByTextAsync(string searchTerm, int? limit = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Product>();

        return await _productRepository.SearchByTextAsync(searchTerm, limit, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return Enumerable.Empty<Product>();

        return await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
    }

    public async Task<PagedList<Product>> GetByCategoryCursorAsync(string categoryId, string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(categoryId), "Category ID cannot be empty") });

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _productRepository.GetProductsByCursorAsync(cursor, pageSize, orderBy, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByMultipleCategoriesAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
    {
        if (categoryIds == null || !categoryIds.Any())
            return Enumerable.Empty<Product>();

        return await _productRepository.GetByMultipleCategoriesAsync(categoryIds, cancellationToken);
    }

    public async Task<(PagedList<Product> Items, long TotalCount)> GetPagedByCategoryAsync(string categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(categoryId), "Category ID cannot be empty") });

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _productRepository.GetPagedByCategoryAsync(categoryId, pageNumber, pageSize, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetBySellerAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            return Enumerable.Empty<Product>();

        return await _productRepository.GetBySellerAsync(sellerId, cancellationToken);
    }

    public async Task<(PagedList<Product> Items, long TotalCount)> GetPagedBySellerAsync(string sellerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(sellerId), "Seller ID cannot be empty") });

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _productRepository.GetPagedBySellerAsync(sellerId, pageNumber, pageSize, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default)
    {
        if (minPrice < 0)
            throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));

        if (maxPrice < 0)
            throw new ArgumentException("Maximum price cannot be negative", nameof(maxPrice));

        if (minPrice > maxPrice)
            throw new ArgumentException("Minimum price cannot be greater than maximum price");

        return await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice, currency, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetBelowPriceAsync(decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default)
    {
        if (maxPrice < 0)
            throw new ArgumentException("Maximum price cannot be negative", nameof(maxPrice));

        return await _productRepository.GetBelowPriceAsync(maxPrice, currency, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAbovePriceAsync(decimal minPrice, string? currency = null, CancellationToken cancellationToken = default)
    {
        if (minPrice < 0)
            throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));

        return await _productRepository.GetAbovePriceAsync(minPrice, currency, cancellationToken);
    }

    public async Task<Dictionary<string, long>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetProductCountByCategoryAsync(cancellationToken);
    }

    public async Task<Dictionary<string, long>> GetProductCountBySellerAsync(CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetProductCountBySellerAsync(cancellationToken);
    }

    public async Task<decimal> GetMinPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetMinPriceAsync(categoryId, currency, cancellationToken);
    }

    public async Task<decimal> GetMaxPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetMaxPriceAsync(categoryId, currency, cancellationToken);
    }

    public async Task<decimal> GetAveragePriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetAveragePriceAsync(categoryId, currency, cancellationToken);
    }

    public async Task<bool> AddCategoryToProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(categoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(categoryId), "Category ID cannot be empty") });

        if (!await _categoryRepository.ExistsAsync(categoryId, cancellationToken))
            throw new NotFoundException($"Category with ID '{categoryId}' does not exist");

        return await _productRepository.AddCategoryToProductAsync(productId, categoryId, cancellationToken);
    }

    public async Task<bool> RemoveCategoryFromProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(categoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(categoryId), "Category ID cannot be empty") });

        return await _productRepository.RemoveCategoryFromProductAsync(productId, categoryId, cancellationToken);
    }

    public async Task<bool> UpdateProductCategoriesAsync(string productId, IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (categoryIds == null)
            throw new ArgumentNullException(nameof(categoryIds));

        var categoryIdsList = categoryIds.ToList();
        if (categoryIdsList.Any())
        {
            var allExist = await _categoryRepository.AllExistAsync(categoryIdsList, cancellationToken);
            if (!allExist)
            {
                var nonExistingIds = await _categoryRepository.GetNonExistingIdsAsync(categoryIdsList, cancellationToken);
                throw new NotFoundException($"Categories do not exist: {string.Join(", ", nonExistingIds)}");
            }
        }

        return await _productRepository.UpdateProductCategoriesAsync(productId, categoryIdsList, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetProductIdsByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return Enumerable.Empty<string>();

        return await _productRepository.GetProductIdsByCategoryAsync(categoryId, cancellationToken);
    }

    public async Task<int> UpdatePriceAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var existingProduct = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
        if (existingProduct == null)
            throw new NotFoundException($"Product with ID '{product.Id}' not found");

        var priceUpdates = new Dictionary<string, decimal> { { product.Id, product.Price.Amount } };
        return await _productRepository.UpdatePricesAsync(priceUpdates, cancellationToken);
    }

    public async Task<int> UpdateSellerAsync(string oldSellerId, string newSellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldSellerId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(oldSellerId), "Old seller ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(newSellerId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(newSellerId), "New seller ID cannot be empty") });

        if (!await _sellerRepository.ExistsAsync(newSellerId, cancellationToken))
            throw new NotFoundException($"New seller with ID '{newSellerId}' does not exist");

        return await _productRepository.UpdateSellerAsync(oldSellerId, newSellerId, cancellationToken);
    }

    public async Task<int> RemoveCategoryFromAllProductsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(categoryId), "Category ID cannot be empty") });

        return await _productRepository.RemoveCategoryFromAllProductsAsync(categoryId, cancellationToken);
    }

    public async Task<int> ReplaceCategoryInAllProductsAsync(string oldCategoryId, string newCategoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldCategoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(oldCategoryId), "Old category ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(newCategoryId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(newCategoryId), "New category ID cannot be empty") });

        if (!await _categoryRepository.ExistsAsync(newCategoryId, cancellationToken))
            throw new NotFoundException($"New category with ID '{newCategoryId}' does not exist");

        return await _productRepository.ReplaceCategoryInAllProductsAsync(oldCategoryId, newCategoryId, cancellationToken);
    }

    public async Task<bool> UpdateProductDetailAsync(string productId, ProductDetail productDetail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (productDetail == null)
            throw new ArgumentNullException(nameof(productDetail));

        return await _productRepository.UpdateProductDetailAsync(productId, productDetail, cancellationToken);
    }

    public async Task<bool> AddImageToProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(imageUrl), "Image URL cannot be empty") });

        return await _productRepository.AddImageToProductAsync(productId, imageUrl, cancellationToken);
    }

    public async Task<bool> RemoveImageFromProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(imageUrl), "Image URL cannot be empty") });

        return await _productRepository.RemoveImageFromProductAsync(productId, imageUrl, cancellationToken);
    }

    public async Task<bool> AddSpecificationAsync(string productId, string key, string value, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(key))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(key), "Specification key cannot be empty") });

        if (string.IsNullOrWhiteSpace(value))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(value), "Specification value cannot be empty") });

        return await _productRepository.AddSpecificationAsync(productId, key, value, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLatestProductsAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        return await _productRepository.GetLatestProductsAsync(count, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsSortedByPriceAsync(bool ascending = true, int? limit = null, CancellationToken cancellationToken = default)
    {
        var pageSize = limit ?? 100;
        return await _productRepository.GetProductsSortedByPriceAsync(1, pageSize, ascending, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsSortedByNameAsync(bool ascending = true, int? limit = null, CancellationToken cancellationToken = default)
    {
        var pageSize = limit ?? 100;
        return await _productRepository.GetProductsSortedByNameAsync(1, pageSize, ascending, cancellationToken);
    }
}