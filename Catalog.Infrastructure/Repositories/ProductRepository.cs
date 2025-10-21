using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

        public ProductRepository(IMongoDatabase database) : base(database)
        {
            _collection = database.GetCollection<Product>("products");
        }

        public async Task<IEnumerable<Product>> SearchByTextAsync(string searchTerm, int? limit = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Product>();

            var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex("Name", new BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex("Description", new BsonRegularExpression(searchTerm, "i"))
            );

            var query = _collection.Find(filter);
            if (limit.HasValue)
                query = query.Limit(limit.Value);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByMultipleCategoriesAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
        {
            var ids = categoryIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            if (!ids.Any()) return Enumerable.Empty<Product>();

            var filter = Builders<Product>.Filter.AnyIn(p => p.Categories, ids);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<(PagedList<Product> Items, long TotalCount)> GetPagedByCategoryAsync(string categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);
            var total = await _collection.CountDocumentsAsync(filter, options: null, cancellationToken: cancellationToken);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (new PagedList<Product>(items, total, pageNumber, pageSize), total);
        }

        public async Task<IEnumerable<Product>> GetBySellerAsync(string sellerId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.SellerId, sellerId);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<(PagedList<Product> Items, long TotalCount)> GetPagedBySellerAsync(string sellerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.SellerId, sellerId);
            var total = await _collection.CountDocumentsAsync(filter, options: null, cancellationToken: cancellationToken);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (new PagedList<Product>(items, total, pageNumber, pageSize), total);
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Gte(p => p.Price.Amount, minPrice),
                Builders<Product>.Filter.Lte(p => p.Price.Amount, maxPrice)
            );

            if (!string.IsNullOrWhiteSpace(currency))
            {
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));
            }

            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetBelowPriceAsync(decimal maxPrice, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Lte(p => p.Price.Amount, maxPrice);
            if (!string.IsNullOrWhiteSpace(currency))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));

            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAbovePriceAsync(decimal minPrice, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Gte(p => p.Price.Amount, minPrice);
            if (!string.IsNullOrWhiteSpace(currency))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));

            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<string, long>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _collection.Distinct<string>("Categories", Builders<Product>.Filter.Empty).ToListAsync(cancellationToken);
            var dict = new Dictionary<string, long>();

            foreach (var catId in categories)
            {
                var count = await _collection.CountDocumentsAsync(Builders<Product>.Filter.AnyEq(p => p.Categories, catId), options: null, cancellationToken: cancellationToken);
                dict[catId] = count;
            }

            return dict;
        }

        public async Task<Dictionary<string, long>> GetProductCountBySellerAsync(CancellationToken cancellationToken = default)
        {
            var sellers = await _collection.Distinct<string>("SellerId", Builders<Product>.Filter.Empty).ToListAsync(cancellationToken);
            var dict = new Dictionary<string, long>();

            foreach (var sellerId in sellers)
            {
                var count = await _collection.CountDocumentsAsync(Builders<Product>.Filter.Eq(p => p.SellerId, sellerId), options: null, cancellationToken: cancellationToken);
                dict[sellerId] = count;
            }

            return dict;
        }

        public async Task<decimal> GetMinPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(categoryId))
                filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);

            if (!string.IsNullOrWhiteSpace(currency))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));

            var sort = Builders<Product>.Sort.Ascending(p => p.Price.Amount);

            var product = await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync(cancellationToken);
            return product?.Price.Amount ?? 0m;
        }

        public async Task<decimal> GetMaxPriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(categoryId))
                filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);

            if (!string.IsNullOrWhiteSpace(currency))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));

            var sort = Builders<Product>.Sort.Descending(p => p.Price.Amount);

            var product = await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync(cancellationToken);
            return product?.Price.Amount ?? 0m;
        }

        public async Task<decimal> GetAveragePriceAsync(string? categoryId = null, string? currency = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(categoryId))
                filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);

            if (!string.IsNullOrWhiteSpace(currency))
                filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Price.Currency, currency));

            var products = await _collection.Find(filter).ToListAsync(cancellationToken);
            if (!products.Any()) return 0m;

            return products.Average(p => p.Price.Amount);
        }

        // Методи для оновлення категорій, цін, зображень, деталей, специфікацій
        public async Task<bool> AddCategoryToProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.AddToSet(p => p.Categories, categoryId);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> RemoveCategoryFromProductAsync(string productId, string categoryId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Pull(p => p.Categories, categoryId);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }
        
        public async Task<bool> UpdateProductCategoriesAsync(string productId, IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Set(p => p.Categories, categoryIds.ToList());
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<IEnumerable<string>> GetProductIdsByCategoryAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);
            var projection = Builders<Product>.Projection.Include(p => p.Id);
            var docs = await _collection.Find(filter).Project<Product>(projection).ToListAsync(cancellationToken);
            return docs.Select(p => p.Id);
        }

        public async Task<int> UpdatePricesAsync(Dictionary<string, decimal> priceUpdates, CancellationToken cancellationToken = default)
        {
            int updatedCount = 0;
            foreach (var kvp in priceUpdates)
            {
                var update = Builders<Product>.Update.Set(p => p.Price.Amount, kvp.Value);
                var res = await _collection.UpdateOneAsync(p => p.Id == kvp.Key, update, cancellationToken: cancellationToken);
                if (res.ModifiedCount > 0) updatedCount++;
            }
            return updatedCount;
        }

        public async Task<int> UpdateSellerAsync(string oldSellerId, string newSellerId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Set(p => p.SellerId, newSellerId);
            var res = await _collection.UpdateManyAsync(p => p.SellerId == oldSellerId, update, cancellationToken: cancellationToken);
            return (int)res.ModifiedCount;
        }

        public async Task<int> RemoveCategoryFromAllProductsAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Pull(p => p.Categories, categoryId);
            var res = await _collection.UpdateManyAsync(Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId), update, cancellationToken: cancellationToken);
            return (int)res.ModifiedCount;
        }

        public async Task<int> ReplaceCategoryInAllProductsAsync(string oldCategoryId, string newCategoryId, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update
                .Pull(p => p.Categories, oldCategoryId)
                .AddToSet(p => p.Categories, newCategoryId);

            var res = await _collection.UpdateManyAsync(Builders<Product>.Filter.AnyEq(p => p.Categories, oldCategoryId), update, cancellationToken: cancellationToken);
            return (int)res.ModifiedCount;
        }

        public async Task<bool> UpdateProductDetailAsync(string productId, ProductDetail productDetail, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Set(p => p.ProductDetail, productDetail);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> AddImageToProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.AddToSet(p => p.ProductDetail.Images, imageUrl);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> RemoveImageFromProductAsync(string productId, string imageUrl, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Pull(p => p.ProductDetail.Images, imageUrl);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> AddSpecificationAsync(string productId, string key, string value, CancellationToken cancellationToken = default)
        {
            var update = Builders<Product>.Update.Set($"ProductDetail.Specifications.{key}", value);
            var res = await _collection.UpdateOneAsync(p => p.Id == productId, update, cancellationToken: cancellationToken);
            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetLatestProductsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(Builders<Product>.Filter.Empty)
                .SortByDescending(p => p.CreatedAt)
                .Limit(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsSortedByPriceAsync(int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sort = ascending ? Builders<Product>.Sort.Ascending(p => p.Price.Amount) : Builders<Product>.Sort.Descending(p => p.Price.Amount);
            return await _collection.Find(Builders<Product>.Filter.Empty)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsSortedByNameAsync(int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sort = ascending ? Builders<Product>.Sort.Ascending(p => p.Name) : Builders<Product>.Sort.Descending(p => p.Name);
            return await _collection.Find(Builders<Product>.Filter.Empty)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Product>> GetProductsByCursorAsync(string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Product>.Filter.Empty;
            if (!string.IsNullOrEmpty(cursor))
                filter = Builders<Product>.Filter.Gt(p => p.Id, cursor);

            var query = _collection.Find(filter)
                .Sort(orderBy == "price" ? Builders<Product>.Sort.Ascending(p => p.Price.Amount) : Builders<Product>.Sort.Ascending(p => p.CreatedAt))
                .Limit(pageSize);

            var items = await query.ToListAsync(cancellationToken);
            var total = await _collection.CountDocumentsAsync(filter, options: null, cancellationToken: cancellationToken);

            return new PagedList<Product>(items, total, 1, pageSize);
        }
}