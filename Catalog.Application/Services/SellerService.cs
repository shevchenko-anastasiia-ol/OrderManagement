using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Services;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IProductRepository _productRepository;

    public SellerService(
        ISellerRepository sellerRepository,
        IProductRepository productRepository)
    {
        _sellerRepository = sellerRepository ?? throw new ArgumentNullException(nameof(sellerRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Seller> CreateAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        if (seller == null)
            throw new ArgumentNullException(nameof(seller));

        // Перевірка унікальності email
        if (!await _sellerRepository.IsEmailUniqueAsync(seller.Email.Value, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Seller with email '{seller.Email.Value}' already exists");

        // Перевірка унікальності телефону
        if (!await _sellerRepository.IsPhoneUniqueAsync(seller.Phone.Value, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Seller with phone '{seller.Phone.Value}' already exists");

        await _sellerRepository.CreateAsync(seller, cancellationToken);
        return seller;
    }

    public async Task<Seller?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _sellerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedList<Seller>> GetAllAsync(SellerParameters sellerParameters, CancellationToken cancellationToken = default)
    {
        if (sellerParameters == null)
            throw new ArgumentNullException(nameof(sellerParameters));

        var (items, totalCount) = await _sellerRepository.GetPagedAsync(
            sellerParameters.PageNumber,
            sellerParameters.PageSize,
            filter: s => !s.IsDeleted,
            orderBy: s => s.CreatedAt,
            ascending: false,
            cancellationToken);

        return new PagedList<Seller>(items.ToList(), (int)totalCount, sellerParameters.PageNumber, sellerParameters.PageSize);
    }

    public async Task<Seller?> UpdateAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        if (seller == null)
            throw new ArgumentNullException(nameof(seller));

        var existing = await _sellerRepository.GetByIdAsync(seller.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Seller with ID '{seller.Id}' not found");

        // Перевірка унікальності email (виключаючи поточного продавця)
        if (!await _sellerRepository.IsEmailUniqueAsync(seller.Email.Value, seller.Id, cancellationToken))
            throw new InvalidOperationException($"Seller with email '{seller.Email.Value}' already exists");

        // Перевірка унікальності телефону (виключаючи поточного продавця)
        if (!await _sellerRepository.IsPhoneUniqueAsync(seller.Phone.Value, seller.Id, cancellationToken))
            throw new InvalidOperationException($"Seller with phone '{seller.Phone.Value}' already exists");

        await _sellerRepository.UpdateAsync(seller, cancellationToken);
        return seller;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Seller ID cannot be empty", nameof(id));

        var seller = await _sellerRepository.GetByIdAsync(id, cancellationToken);
        if (seller == null)
            return false;

        // Перевірка чи є у продавця продукти
        var hasProducts = await _sellerRepository.HasProductsAsync(id, cancellationToken);
        if (hasProducts)
        {
            var productCount = await _sellerRepository.GetProductCountAsync(id, cancellationToken);
            throw new InvalidOperationException(
                $"Cannot delete seller '{seller.Name}' because they have {productCount} products. " +
                "Please reassign or delete the products first.");
        }

        // Soft delete
        await _sellerRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Seller>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Enumerable.Empty<Seller>();

        return await _sellerRepository.SearchByNameAsync(name, cancellationToken);
    }

    public async Task<Seller?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _sellerRepository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _sellerRepository.IsEmailUniqueAsync(email, excludeId, cancellationToken);
    }

    public async Task<Seller?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        return await _sellerRepository.GetByPhoneAsync(phone, cancellationToken);
    }

    public async Task<bool> IsPhoneUniqueAsync(string phone, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        return await _sellerRepository.IsPhoneUniqueAsync(phone, excludeId, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            return false;

        return await _sellerRepository.HasProductsAsync(sellerId, cancellationToken);
    }

    public async Task<long> GetProductCountAsync(string sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            return 0;

        return await _sellerRepository.GetProductCountAsync(sellerId, cancellationToken);
    }

    public async Task<IEnumerable<Seller>> GetByIdsAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default)
    {
        if (sellerIds == null || !sellerIds.Any())
            return Enumerable.Empty<Seller>();

        return await _sellerRepository.GetByIdsAsync(sellerIds, cancellationToken);
    }

    public async Task<bool> AllExistAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default)
    {
        if (sellerIds == null || !sellerIds.Any())
            return false;

        return await _sellerRepository.AllExistAsync(sellerIds, cancellationToken);
    }

    public async Task<bool> UpdateEmailAsync(string sellerId, string newEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            throw new ArgumentException("Seller ID cannot be empty", nameof(sellerId));

        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty", nameof(newEmail));

        var seller = await _sellerRepository.GetByIdAsync(sellerId, cancellationToken);
        if (seller == null)
            throw new InvalidOperationException($"Seller with ID '{sellerId}' not found");

        // Перевірка унікальності нового email
        if (!await _sellerRepository.IsEmailUniqueAsync(newEmail, sellerId, cancellationToken))
            throw new InvalidOperationException($"Email '{newEmail}' is already in use");

        // Валідація email через Value Object
        var email = new Email(newEmail);
        
        return await _sellerRepository.UpdateEmailAsync(sellerId, newEmail, cancellationToken);
    }

    public async Task<bool> UpdatePhoneAsync(string sellerId, string newPhone, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
            throw new ArgumentException("Seller ID cannot be empty", nameof(sellerId));

        if (string.IsNullOrWhiteSpace(newPhone))
            throw new ArgumentException("Phone cannot be empty", nameof(newPhone));

        var seller = await _sellerRepository.GetByIdAsync(sellerId, cancellationToken);
        if (seller == null)
            throw new InvalidOperationException($"Seller with ID '{sellerId}' not found");

        // Перевірка унікальності нового телефону
        if (!await _sellerRepository.IsPhoneUniqueAsync(newPhone, sellerId, cancellationToken))
            throw new InvalidOperationException($"Phone '{newPhone}' is already in use");

        // Валідація телефону через Value Object
        var phone = new Phone(newPhone);
        
        return await _sellerRepository.UpdatePhoneAsync(sellerId, newPhone, cancellationToken);
    }

    public async Task<PagedList<Seller>> GetAllSortedByNameAsync(
        int pageNumber, 
        int pageSize, 
        bool ascending = true, 
        string? orderBy = null, 
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _sellerRepository.GetAllSortedByNameAsync(
            pageNumber, 
            pageSize, 
            ascending, 
            orderBy, 
            cancellationToken);
    }

    public async Task<IEnumerable<Seller>> GetRecentSellersAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count < 1)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        return await _sellerRepository.GetRecentSellersAsync(count, cancellationToken);
    }

    public async Task<PagedList<Seller>> GetByCursorAsync(
        string? cursor, 
        int pageSize, 
        string? orderBy = null, 
        CancellationToken cancellationToken = default)
    {
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        return await _sellerRepository.GetByCursorAsync(cursor, pageSize, orderBy, cancellationToken);
    }
}