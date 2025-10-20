using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces.Services;

public interface IReviewService
    {
        Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default);
        Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Review?> UpdateAsync(Review review, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Review>> GetByProductAsync(string productId, CancellationToken cancellationToken = default);
        Task<(PagedList<Review> Items, long TotalCount)> GetPagedByProductAsync(string productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<long> GetProductReviewCountAsync(string productId, CancellationToken cancellationToken = default);
        Task<bool> HasUserReviewedProductAsync(string productId, string author, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByAuthorAsync(string author, CancellationToken cancellationToken = default);
        Task<(PagedList<Review> Items, long TotalCount)> GetPagedByAuthorAsync(string author, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<long> GetAuthorReviewCountAsync(string author, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByRatingAsync(int rating, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByRatingRangeAsync(int minRating, int maxRating, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetProductReviewsByRatingAsync(string productId, int rating, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetHighRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetLowRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default);
        Task<double> GetAverageRatingAsync(string productId, CancellationToken cancellationToken = default);
        Task<double> GetSellerAverageRatingAsync(string sellerId, CancellationToken cancellationToken = default);
        Task<Dictionary<int, long>> GetRatingDistributionAsync(string productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> SearchInCommentsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> SearchProductReviewsAsync(string productId, string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> AddReplyToReviewAsync(string reviewId, string author, string text, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetReviewsWithRepliesAsync(string? productId = null, CancellationToken cancellationToken = default);
        Task<int> GetReplyCountAsync(string reviewId, CancellationToken cancellationToken = default);
        Task<bool> RemoveAllRepliesAsync(string reviewId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByIdsAsync(IEnumerable<string> reviewIds, CancellationToken cancellationToken = default);
        Task<long> DeleteAllProductReviewsAsync(string productId, CancellationToken cancellationToken = default);
        Task<long> DeleteAllAuthorReviewsAsync(string author, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetMostRecentReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetMostHelpfulReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string? productId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetRecentReviewsAsync(int days, string? productId = null, CancellationToken cancellationToken = default);
    }