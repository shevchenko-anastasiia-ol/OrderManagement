using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IMongoCollection<Category> _collection;
        private readonly IMongoCollection<Product> _productCollection;

        public CategoryRepository(IMongoDatabase database) : base(database)
        {
            _collection = database.GetCollection<Category>("categories");
            _productCollection = database.GetCollection<Product>("products"); // для підрахунку продуктів
        }

        public async Task<IEnumerable<Category>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Array.Empty<Category>();

            var filter = Builders<Category>.Filter.Regex("Name", new BsonRegularExpression(name, "i"));
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var filter = Builders<Category>.Filter.Eq("Name", name);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> IsCategoryInUseAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return false;

            var filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);
            var count = await _productCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var filter = Builders<Category>.Filter.Eq(c => c.Name, name);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                var excludeFilter = Builders<Category>.Filter.Ne(c => c.Id, excludeId);
                filter = Builders<Category>.Filter.And(filter, excludeFilter);
            }

            var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<long> GetProductCountAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return 0;

            var filter = Builders<Product>.Filter.AnyEq(p => p.Categories, categoryId);
            return await _productCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
        {
            var ids = categoryIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            if (!ids.Any()) return Array.Empty<Category>();

            var filter = Builders<Category>.Filter.In(c => c.Id, ids);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<bool> AllExistAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
        {
            var ids = categoryIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            if (!ids.Any()) return true;

            var filter = Builders<Category>.Filter.In(c => c.Id, ids);
            var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count == ids.Count;
        }

        public async Task<IEnumerable<string>> GetNonExistingIdsAsync(IEnumerable<string> categoryIds, CancellationToken cancellationToken = default)
        {
            var ids = categoryIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            if (!ids.Any()) return Array.Empty<string>();

            var filter = Builders<Category>.Filter.In(c => c.Id, ids);
            var existing = await _collection.Find(filter).Project(c => c.Id).ToListAsync(cancellationToken);
            return ids.Except(existing);
        }

        public async Task<IEnumerable<Category>> GetEmptyCategoriesAsync(CancellationToken cancellationToken = default)
        {
            // Категорії, в яких немає продуктів
            var categories = await _collection.Find(Builders<Category>.Filter.Empty).ToListAsync(cancellationToken);
            var result = new List<Category>();

            foreach (var cat in categories)
            {
                var count = await GetProductCountAsync(cat.Id, cancellationToken);
                if (count == 0)
                    result.Add(cat);
            }

            return result;
        }

        public async Task<PagedList<Category>> GetAllSortedByNameAsync(int pageNumber, int pageSize, bool ascending = true, string? orderBy = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Category>.Filter.Empty;
            var sort = ascending 
                ? Builders<Category>.Sort.Ascending(c => c.Name) 
                : Builders<Category>.Sort.Descending(c => c.Name);

            var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            var items = await _collection.Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<Category>(items, total, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> GetRecentCategoriesAsync(int count, CancellationToken cancellationToken = default)
        {
            var sort = Builders<Category>.Sort.Descending("_id"); // припускаємо що _id по порядку вставки
            return await _collection.Find(Builders<Category>.Filter.Empty)
                .Sort(sort)
                .Limit(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Category>> GetByCursorAsync(string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Category>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                // cursor як Id останнього елемента
                filter = Builders<Category>.Filter.Gt(c => c.Id, cursor);
            }

            var sort = Builders<Category>.Sort.Ascending(orderBy ?? "_id");
            var items = await _collection.Find(filter)
                .Sort(sort)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var total = await _collection.CountDocumentsAsync(c => true, cancellationToken: cancellationToken);


            return new PagedList<Category>(items, total, 1, pageSize);
        }
    }