using MongoDB.Driver;
using System.Linq.Expressions;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Common.Helpers;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories;

public class SellerRepository : GenericRepository<Seller>, ISellerRepository
{
    private readonly MongoDbContext _context;

    public SellerRepository(MongoDbContext context) 
        : base(context.Database, "Sellers")
    {
        _context = context;
    }

    // Override базових методів для підтримки soft delete
    public new async Task<Seller?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public new async Task<IEnumerable<Seller>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(s => !s.IsDeleted, cancellationToken);
    }

    public async Task<PagedList<Seller>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await base.GetPagedAsync(
            pageNumber, 
            pageSize, 
            filter: s => !s.IsDeleted,
            orderBy: s => s.CreatedAt,
            ascending: false,
            cancellationToken
        );

        return new PagedList<Seller>(items.ToList(), (int)totalCount, pageNumber, pageSize);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await CountAsync(s => !s.IsDeleted, cancellationToken);
    }

    public new async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await CountAsync(s => s.Id == id && !s.IsDeleted, cancellationToken) > 0;
    }

    public async Task AddAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        await CreateAsync(seller, cancellationToken);
    }

    public async Task UpdateAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(seller, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var seller = await GetByIdAsync(id, cancellationToken);
        if (seller != null)
        {
            seller.MarkAsDeleted();
            await UpdateAsync(seller, cancellationToken);
        }
    }

    public async Task HardDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await base.DeleteAsync(id, cancellationToken);
    }

    // ISellerRepository specific methods
    public async Task<IEnumerable<Seller>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Seller>.Filter.And(
            Builders<Seller>.Filter.Text(name),
            Builders<Seller>.Filter.Eq(s => s.IsDeleted, false)
        );

        return await _context.Sellers
            .Find(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<Seller?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(
            s => s.Email.Value == email.ToLowerInvariant() && !s.IsDeleted, 
            cancellationToken
        );
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        Expression<Func<Seller, bool>> filter = string.IsNullOrEmpty(excludeId)
            ? s => s.Email.Value == email.ToLowerInvariant() && !s.IsDeleted
            : s => s.Email.Value == email.ToLowerInvariant() && s.Id != excludeId && !s.IsDeleted;

        var count = await CountAsync(filter, cancellationToken);
        return count == 0;
    }

    public async Task<Seller?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var cleanedPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        return await FindOneAsync(
            s => s.Phone.Value == cleanedPhone && !s.IsDeleted, 
            cancellationToken
        );
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var cleanedPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        Expression<Func<Seller, bool>> filter = string.IsNullOrEmpty(excludeId)
            ? s => s.Phone.Value == cleanedPhone && !s.IsDeleted
            : s => s.Phone.Value == cleanedPhone && s.Id != excludeId && !s.IsDeleted;

        var count = await CountAsync(filter, cancellationToken);
        return count == 0;
    }

    public async Task<bool> HasProductsAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Find(p => p.SellerId == sellerId && !p.IsDeleted)
            .AnyAsync(cancellationToken);
    }

    public async Task<long> GetProductCountAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .CountDocumentsAsync(
                p => p.SellerId == sellerId && !p.IsDeleted, 
                cancellationToken: cancellationToken
            );
    }

    public async Task<IEnumerable<Seller>> GetByIdsAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default)
    {
        var idList = sellerIds.ToList();
        return await FindAsync(s => idList.Contains(s.Id) && !s.IsDeleted, cancellationToken);
    }

    public async Task<bool> AllExistAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default)
    {
        var sellerIdsList = sellerIds.ToList();
        var count = await CountAsync(
            s => sellerIdsList.Contains(s.Id) && !s.IsDeleted,
            cancellationToken
        );

        return count == sellerIdsList.Count;
    }

    public async Task<bool> UpdateEmailAsync(string sellerId, string newEmail, CancellationToken cancellationToken = default)
    {
        var seller = await GetByIdAsync(sellerId, cancellationToken);
        if (seller == null)
            return false;

        seller.SetEmail(new Email(newEmail));
        await UpdateAsync(seller, cancellationToken);
        return true;
    }

    public async Task<bool> UpdatePhoneAsync(string sellerId, string newPhone, CancellationToken cancellationToken = default)
    {
        var seller = await GetByIdAsync(sellerId, cancellationToken);
        if (seller == null)
            return false;

        seller.SetPhone(new Phone(newPhone));
        await UpdateAsync(seller, cancellationToken);
        return true;
    }

    public async Task<PagedList<Seller>> GetAllSortedByNameAsync(
        int pageNumber, 
        int pageSize, 
        bool ascending = true, 
        string? orderBy = null, 
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Seller, object>>? orderByExpression = orderBy?.ToLower() switch
        {
            "name" => s => s.Name,
            "createdat" => s => s.CreatedAt,
            _ => s => s.Name
        };

        var (items, totalCount) = await base.GetPagedAsync(
            pageNumber,
            pageSize,
            filter: s => !s.IsDeleted,
            orderBy: orderByExpression,
            ascending: ascending,
            cancellationToken
        );

        return new PagedList<Seller>(items.ToList(), (int)totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<Seller>> GetRecentSellersAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Sellers
            .Find(s => !s.IsDeleted)
            .SortByDescending(s => s.CreatedAt)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<Seller>> GetByCursorAsync(
        string? cursor, 
        int pageSize, 
        string? orderBy = null, 
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Seller>.Filter;
        var filter = filterBuilder.Eq(s => s.IsDeleted, false);

        // Якщо є курсор, додаємо фільтр для пагінації
        if (!string.IsNullOrEmpty(cursor))
        {
            filter = filterBuilder.And(
                filter,
                filterBuilder.Gt(s => s.Id, cursor)
            );
        }

        var findFluent = _context.Sellers.Find(filter);

        // Сортування
        SortDefinition<Seller> sortDefinition = orderBy?.ToLower() switch
        {
            "name" => Builders<Seller>.Sort.Ascending(s => s.Name).Ascending(s => s.Id),
            "email" => Builders<Seller>.Sort.Ascending("email.value").Ascending(s => s.Id),
            "createdat" => Builders<Seller>.Sort.Descending(s => s.CreatedAt).Ascending(s => s.Id),
            _ => Builders<Seller>.Sort.Ascending(s => s.Id)
        };

        findFluent = findFluent.Sort(sortDefinition);

        var items = await findFluent
            .Limit(pageSize + 1) // +1 щоб визначити, чи є ще елементи
            .ToListAsync(cancellationToken);

        var hasMore = items.Count > pageSize;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);
        }

        var totalCount = await CountAsync(s => !s.IsDeleted, cancellationToken);

        // Для cursor-based pagination pageNumber не має сенсу, але передаємо 1
        return new PagedList<Seller>(items, (int)totalCount, 1, pageSize);
    }
}