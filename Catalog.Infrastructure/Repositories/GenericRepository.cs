using System.Linq.Expressions;
using System.Reflection;
using Catalog.Domain.Common;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly PropertyInfo? _versionProperty; 

        public GenericRepository(IMongoDatabase database, string? collectionName = null)
        {
            if (database is null) throw new ArgumentNullException(nameof(database));

            var name = collectionName ?? typeof(TEntity).Name.ToLowerInvariant() + "s";
            _collection = database.GetCollection<TEntity>(name);

            // Шукаємо property "Version" або "RowVersion" (case-insensitive) для concurrency
            _versionProperty = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, "Version", StringComparison.OrdinalIgnoreCase)
                                     || string.Equals(p.Name, "RowVersion", StringComparison.OrdinalIgnoreCase));
        }

        private FilterDefinition<TEntity> BuildFilter(Expression<Func<TEntity, bool>>? filter)
        {
            return filter is null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(filter);
        }

        public async Task<TEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(Builders<TEntity>.Filter.Empty).ToListAsync(cancellationToken);
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var f = BuildFilter(filter);
            return await _collection.Find(f).ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var f = BuildFilter(filter);
            return await _collection.Find(f).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            var f = BuildFilter(filter);
            return await _collection.CountDocumentsAsync(f, cancellationToken: cancellationToken);
        }

        public async Task<(IEnumerable<TEntity> Items, long TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var f = BuildFilter(filter);
            var total = await _collection.CountDocumentsAsync(f, cancellationToken: cancellationToken);

            var find = _collection.Find(f);

            // Сортування
            if (orderBy is not null)
            {
                var sortBuilder = Builders<TEntity>.Sort;
                var sortDef = ascending
                    ? sortBuilder.Ascending(orderBy)
                    : sortBuilder.Descending(orderBy);
                find = find.Sort(sortDef);
            }
            else
            {
                // За замовчуванням сортуємо по Id (якщо існує)
                find = find.Sort(ascending ? Builders<TEntity>.Sort.Ascending("_id") : Builders<TEntity>.Sort.Descending("_id"));
            }

            var skip = Math.Max(0, (pageNumber - 1)) * Math.Max(1, pageSize);
            var items = await find.Skip(skip).Limit(pageSize).ToListAsync(cancellationToken);
            return (items, (long)total);
        }

        public async Task<IEnumerable<TEntity>> CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var list = (entities ?? Array.Empty<TEntity>()).ToList();
            if (!list.Any()) return list;
            await _collection.InsertManyAsync(list, cancellationToken: cancellationToken);
            return list;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var result = await _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("Id", id), cancellationToken);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var f = BuildFilter(filter);
            var result = await _collection.DeleteManyAsync(f, cancellationToken);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var count = await _collection.CountDocumentsAsync(Builders<TEntity>.Filter.Eq("Id", id), cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Id)) throw new ArgumentException("Entity Id is required for update", nameof(entity));

            var filter = Builders<TEntity>.Filter.Eq("Id", entity.Id);
            var res = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
            if (!res.IsAcknowledged || res.ModifiedCount == 0)
            {
                throw new InvalidOperationException($"Failed to update document with Id = {entity.Id}");
            }
            return entity;
        }


        public async Task<TEntity> UpdateWithConcurrencyCheckAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Id)) throw new ArgumentException("Entity Id is required for update", nameof(entity));

            if (_versionProperty is null)
            {
                await UpdateAsync(entity, cancellationToken);
                return entity;
            }
            
            var currentVersionObj = _versionProperty.GetValue(entity);
            if (currentVersionObj is null) throw new InvalidOperationException("Version property is null on entity.");

            if (!int.TryParse(currentVersionObj.ToString(), out var currentVersion))
                throw new InvalidOperationException("Version property is not an int.");
            
            var filter = Builders<TEntity>.Filter.And(
                Builders<TEntity>.Filter.Eq("Id", entity.Id),
                Builders<TEntity>.Filter.Eq(_versionProperty.Name, currentVersion)
            );
            
            var nextVersion = currentVersion + 1;
            _versionProperty.SetValue(entity, nextVersion);


            var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);

            if (!result.IsAcknowledged || result.ModifiedCount == 0)
            {
                _versionProperty.SetValue(entity, currentVersion);
                throw new InvalidOperationException("Concurrency conflict: entity version mismatch.");
            }

            return entity;
        }


        public async Task<(bool Success, TEntity? Entity)> TryUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_versionProperty is null)
                {
                    await UpdateAsync(entity, cancellationToken);
                    return (true, entity);
                }

                var currentVersionObj = _versionProperty.GetValue(entity);
                if (currentVersionObj is null) return (false, null);
                if (!int.TryParse(currentVersionObj.ToString(), out var currentVersion)) return (false, null);

                var filter = Builders<TEntity>.Filter.And(
                    Builders<TEntity>.Filter.Eq("Id", entity.Id),
                    Builders<TEntity>.Filter.Eq(_versionProperty.Name, currentVersion)
                );

                var nextVersion = currentVersion + 1;
                _versionProperty.SetValue(entity, nextVersion);

                var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
                if (!result.IsAcknowledged || result.ModifiedCount == 0)
                {
                    _versionProperty.SetValue(entity, currentVersion);
                    return (false, null);
                }

                return (true, entity);
            }
            catch
            {
                return (false, null);
            }
        }
    }