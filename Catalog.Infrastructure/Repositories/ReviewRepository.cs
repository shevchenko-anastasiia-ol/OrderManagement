using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMongoCollection<Review> _collection;

    public ReviewRepository(IMongoDatabase database, string? collectionName = null) 
        : base(database, collectionName ?? "review")
    {
        _collection = database.GetCollection<Review>(collectionName ?? "review");
    }

    public async Task<IEnumerable<Review>> GetByProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.ProductId == productId).ToListAsync(cancellationToken);
    }

    public async Task<(PagedList<Review> Items, long TotalCount)> GetPagedByProductAsync(string productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _collection.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return (new PagedList<Review>(items, total, pageNumber, pageSize), total);
    }

    public async Task<long> GetProductReviewCountAsync(string productId, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(r => r.ProductId == productId, cancellationToken: cancellationToken);
    }

    public async Task<bool> HasUserReviewedProductAsync(string productId, string author, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.ProductId == productId && r.Author == author).AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.Author == author).ToListAsync(cancellationToken);
    }

    public async Task<(PagedList<Review> Items, long TotalCount)> GetPagedByAuthorAsync(string author, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.Author, author);
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _collection.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .SortByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return (new PagedList<Review>(items, total, pageNumber, pageSize), total);
    }

    public async Task<long> GetAuthorReviewCountAsync(string author, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(r => r.Author == author, cancellationToken: cancellationToken);
    }

    public async Task<bool> AddReplyToReviewAsync(string reviewId, string author, string text, CancellationToken cancellationToken = default)
    {
        var reply = new ReviewReply( author,  text);
        var update = Builders<Review>.Update.AddToSet(r => r.Replies, reply);
        var res = await _collection.UpdateOneAsync(r => r.Id == reviewId, update, cancellationToken: cancellationToken);
        return res.IsAcknowledged && res.ModifiedCount > 0;
    }

    public async Task<IEnumerable<Review>> GetReviewsWithRepliesAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        var filter = productId != null
            ? Builders<Review>.Filter.Eq(r => r.ProductId, productId) & Builders<Review>.Filter.Ne(r => r.Replies, null)
            : Builders<Review>.Filter.Ne(r => r.Replies, null);

        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<int> GetReplyCountAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _collection.Find(r => r.Id == reviewId).FirstOrDefaultAsync(cancellationToken);
        return review?.Replies.Count ?? 0;
    }

    public async Task<bool> RemoveAllRepliesAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Review>.Update.Set(r => r.Replies, new List<ReviewReply>());
        var res = await _collection.UpdateOneAsync(r => r.Id == reviewId, update, cancellationToken: cancellationToken);
        return res.IsAcknowledged && res.ModifiedCount > 0;
    }

    public async Task<IEnumerable<Review>> GetByIdsAsync(IEnumerable<string> reviewIds, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.In(r => r.Id, reviewIds);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<long> DeleteAllProductReviewsAsync(string productId, CancellationToken cancellationToken = default)
    {
        var res = await _collection.DeleteManyAsync(r => r.ProductId == productId, cancellationToken);
        return res.DeletedCount;
    }

    public async Task<long> DeleteAllAuthorReviewsAsync(string author, CancellationToken cancellationToken = default)
    {
        var res = await _collection.DeleteManyAsync(r => r.Author == author, cancellationToken);
        return res.DeletedCount;
    }
    public async Task<IEnumerable<Review>> GetMostRecentReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default)
    {
        var filter = productId != null
            ? Builders<Review>.Filter.Eq(r => r.ProductId, productId)
            : Builders<Review>.Filter.Empty;

        return await _collection.Find(filter)
            .SortByDescending(r => r.CreatedAt)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetMostHelpfulReviewsAsync(int count, string? productId = null, CancellationToken cancellationToken = default)
    {
        // Для прикладу використовуємо Replies.Count як критерій "helpfulness"
        var filter = productId != null
            ? Builders<Review>.Filter.Eq(r => r.ProductId, productId)
            : Builders<Review>.Filter.Empty;

        return await _collection.Find(filter)
            .SortByDescending(r => r.Replies.Count)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string? productId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Gte(r => r.CreatedAt, startDate) &
                     Builders<Review>.Filter.Lte(r => r.CreatedAt, endDate);

        if (!string.IsNullOrEmpty(productId))
            filter &= Builders<Review>.Filter.Eq(r => r.ProductId, productId);

        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int days, string? productId = null, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        var filter = Builders<Review>.Filter.Gte(r => r.CreatedAt, cutoff);

        if (!string.IsNullOrEmpty(productId))
            filter &= Builders<Review>.Filter.Eq(r => r.ProductId, productId);

        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<PagedList<Review>> GetByCursorAsync(
        string? cursor,
        int pageSize,
        string? productId = null,
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {

        var filter = string.IsNullOrEmpty(productId)
            ? Builders<Review>.Filter.Empty
            : Builders<Review>.Filter.Eq(r => r.ProductId, productId);


        if (!string.IsNullOrEmpty(cursor))
        {
            var cursorFilter = Builders<Review>.Filter.Gt(r => r.Id, cursor);
            filter = Builders<Review>.Filter.And(filter, cursorFilter);
        }


        var query = _collection.Find(filter);
        
        if (!string.IsNullOrEmpty(orderBy))
        {
            query = query.Sort(Builders<Review>.Sort.Ascending(orderBy));
        }
        else
        {
            query = query.Sort(Builders<Review>.Sort.Ascending(r => r.Id));
        }
        
        var items = await query.Limit(pageSize).ToListAsync(cancellationToken);
        
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return new PagedList<Review>(items, total, 1, pageSize);
    }

    
    public async Task<IEnumerable<Review>> GetByRatingAsync(int rating, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.Rating == rating).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Review>> GetByRatingRangeAsync(int minRating, int maxRating, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.Rating >= minRating && r.Rating <= maxRating).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Review>> GetProductReviewsByRatingAsync(string productId, int rating, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.ProductId == productId && r.Rating == rating).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Review>> GetHighRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        var filter = productId != null
            ? Builders<Review>.Filter.Eq(r => r.ProductId, productId) & Builders<Review>.Filter.Gte(r => r.Rating, 4)
            : Builders<Review>.Filter.Gte(r => r.Rating, 4);
    
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Review>> GetLowRatedReviewsAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        var filter = productId != null
            ? Builders<Review>.Filter.Eq(r => r.ProductId, productId) & Builders<Review>.Filter.Lte(r => r.Rating, 2)
            : Builders<Review>.Filter.Lte(r => r.Rating, 2);
    
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }
    
    public async Task<Dictionary<int, long>> GetRatingDistributionAsync(string productId, CancellationToken cancellationToken = default)
    {
        var reviews = await _collection.Find(r => r.ProductId == productId).ToListAsync(cancellationToken);
        return reviews.GroupBy(r => r.Rating)
                      .ToDictionary(g => g.Key, g => (long)g.Count());
    }
    
    public async Task<double> GetAverageRatingAsync(string productId, CancellationToken cancellationToken = default)
    {
        var reviews = await _collection.Find(r => r.ProductId == productId).ToListAsync(cancellationToken);
        return reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;
    }
    
    public async Task<double> GetSellerAverageRatingAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        var products = await _productCollection.Find(p => p.SellerId == sellerId)
            .Project(p => p.Id)
            .ToListAsync(cancellationToken);
        
        var reviews = await _collection.Find(r => products.Contains(r.ProductId))
            .ToListAsync(cancellationToken);
        return reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;
    }
    
    // --- Search methods ---
    public async Task<IEnumerable<Review>> SearchInCommentsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.Comment.Contains(searchTerm)).ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Review>> SearchProductReviewsAsync(string productId, string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(r => r.ProductId == productId && r.Comment.Contains(searchTerm)).ToListAsync(cancellationToken);
    }
    
    /*public IFindFluent<Review, Review> FilterByCursor(this IFindFluent<Review, Review> query, string cursor, string? orderBy)
    {
        var objectId = new ObjectId(cursor); // cursor має бути рядком, який перетворюється в ObjectId
        var filter = Builders<Review>.Filter.Gt(r => r.Id, objectId); // > cursor
        var query = _collection.Find(filter)
            .Sort(Builders<Review>.Sort.Ascending(r => r.Id))
            .Limit(pageSize);

        var results = await query.ToListAsync(cancellationToken);
        return results;
    }*/
}

public static class MongoCursorExtensions
{
    
}