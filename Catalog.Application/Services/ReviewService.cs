using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.Exceptions;
using FluentValidation.Results;

namespace Catalog.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default)
    {
        if (review == null)
            throw new ArgumentNullException(nameof(review));

        var productExists = await _productRepository.ExistsAsync(review.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException($"Product with ID '{review.ProductId}' does not exist");

        var hasReviewed = await _reviewRepository.HasUserReviewedProductAsync(review.ProductId, review.Author, cancellationToken);
        if (hasReviewed)
            throw new ConflictException($"User '{review.Author}' has already reviewed this product");

        return await _reviewRepository.CreateAsync(review, cancellationToken);
    }

    public async Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _reviewRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedList<Review>> GetAllAsync(ReviewParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var (items, totalCount) = await _reviewRepository.GetPagedAsync(
            parameters.PageNumber,
            parameters.PageSize,
            filter: null,
            orderBy: r => r.CreatedAt,
            ascending: false,
            cancellationToken);

        return new PagedList<Review>(items.ToList(), (int)totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Review?> UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        if (review == null)
            throw new ArgumentNullException(nameof(review));

        var existing = await _reviewRepository.GetByIdAsync(review.Id, cancellationToken);
        if (existing == null)
            throw new NotFoundException($"Review with ID '{review.Id}' not found");

        return await _reviewRepository.UpdateAsync(review, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(id), "Review ID cannot be empty") });

        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review == null)
            return false;

        return await _reviewRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return Enumerable.Empty<Review>();

        return await _reviewRepository.GetByProductAsync(productId, cancellationToken);
    }

    public async Task<(PagedList<Review> Items, long TotalCount)> GetPagedByProductAsync(string productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _reviewRepository.GetPagedByProductAsync(productId, pageNumber, pageSize, cancellationToken);
    }

    public async Task<long> GetProductReviewCountAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return 0;

        return await _reviewRepository.GetProductReviewCountAsync(productId, cancellationToken);
    }

    public async Task<bool> HasUserReviewedProductAsync(string productId, string author, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(author))
            return false;

        return await _reviewRepository.HasUserReviewedProductAsync(productId, author, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(author))
            return Enumerable.Empty<Review>();

        return await _reviewRepository.GetByAuthorAsync(author, cancellationToken);
    }

    public async Task<(PagedList<Review> Items, long TotalCount)> GetPagedByAuthorAsync(string author, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(author), "Author cannot be empty") });

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _reviewRepository.GetPagedByAuthorAsync(author, pageNumber, pageSize, cancellationToken);
    }

    public async Task<long> GetAuthorReviewCountAsync(string author, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(author))
            return 0;

        return await _reviewRepository.GetAuthorReviewCountAsync(author, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByRatingAsync(int rating, CancellationToken cancellationToken = default)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        return await _reviewRepository.GetByRatingAsync(rating, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByRatingRangeAsync(int minRating, int maxRating, CancellationToken cancellationToken = default)
    {
        if (minRating < 1 || minRating > 5)
            throw new ArgumentException("Minimum rating must be between 1 and 5", nameof(minRating));

        if (maxRating < 1 || maxRating > 5)
            throw new ArgumentException("Maximum rating must be between 1 and 5", nameof(maxRating));

        if (minRating > maxRating)
            throw new ArgumentException("Minimum rating cannot be greater than maximum rating");

        return await _reviewRepository.GetByRatingRangeAsync(minRating, maxRating, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetProductReviewsByRatingAsync(string productId, int rating, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        return await _reviewRepository.GetProductReviewsByRatingAsync(productId, rating, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetHighRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        return await _reviewRepository.GetHighRatedReviewsAsync(productId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetLowRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        return await _reviewRepository.GetLowRatedReviewsAsync(productId, cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return 0;

        return await _reviewRepository.GetAverageRatingAsync(productId, cancellationToken);
    }

    public async Task<double> GetSellerAverageRatingAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            return 0;

        return await _reviewRepository.GetSellerAverageRatingAsync(sellerId, cancellationToken);
    }

    public async Task<Dictionary<int, long>> GetRatingDistributionAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        return await _reviewRepository.GetRatingDistributionAsync(productId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> SearchInCommentsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Review>();

        return await _reviewRepository.SearchInCommentsAsync(searchTerm, cancellationToken);
    }

    public async Task<IEnumerable<Review>> SearchProductReviewsAsync(string productId, string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Review>();

        return await _reviewRepository.SearchProductReviewsAsync(productId, searchTerm, cancellationToken);
    }

    public async Task<bool> AddReplyToReviewAsync(string reviewId, string author, string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reviewId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(reviewId), "Review ID cannot be empty") });

        if (string.IsNullOrWhiteSpace(author))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(author), "Author cannot be empty") });

        if (string.IsNullOrWhiteSpace(text))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(text), "Reply text cannot be empty") });

        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review == null)
            throw new NotFoundException($"Review with ID '{reviewId}' not found");

        return await _reviewRepository.AddReplyToReviewAsync(reviewId, author, text, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetReviewsWithRepliesAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        return await _reviewRepository.GetReviewsWithRepliesAsync(productId, cancellationToken);
    }

    public async Task<int> GetReplyCountAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reviewId))
            return 0;

        return await _reviewRepository.GetReplyCountAsync(reviewId, cancellationToken);
    }

    public async Task<bool> RemoveAllRepliesAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reviewId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(reviewId), "Review ID cannot be empty") });

        var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review == null)
            throw new NotFoundException($"Review with ID '{reviewId}' not found");

        return await _reviewRepository.RemoveAllRepliesAsync(reviewId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByIdsAsync(IEnumerable<string> reviewIds, CancellationToken cancellationToken = default)
    {
        if (reviewIds == null || !reviewIds.Any())
            return Enumerable.Empty<Review>();

        return await _reviewRepository.GetByIdsAsync(reviewIds, cancellationToken);
    }

    public async Task<long> DeleteAllProductReviewsAsync(string productId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(productId), "Product ID cannot be empty") });

        return await _reviewRepository.DeleteAllProductReviewsAsync(productId, cancellationToken);
    }

    public async Task<long> DeleteAllAuthorReviewsAsync(string author, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new CustomValidationException(new[] { new ValidationFailure(nameof(author), "Author cannot be empty") });

        return await _reviewRepository.DeleteAllAuthorReviewsAsync(author, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetMostRecentReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        return await _reviewRepository.GetMostRecentReviewsAsync(count, productId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetMostHelpfulReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        return await _reviewRepository.GetMostHelpfulReviewsAsync(count, productId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string? productId = null, CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date");

        return await _reviewRepository.GetByDateRangeAsync(startDate, endDate, productId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int days, string? productId = null, CancellationToken cancellationToken = default)
    {
        if (days < 1)
            throw new ArgumentException("Days must be greater than 0", nameof(days));

        return await _reviewRepository.GetRecentReviewsAsync(days, productId, cancellationToken);
    }
}