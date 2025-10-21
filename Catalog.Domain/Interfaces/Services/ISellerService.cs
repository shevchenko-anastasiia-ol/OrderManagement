using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Domain.Interfaces.Services;

public interface ISellerService
    {
        Task<Seller> CreateAsync(Seller seller, CancellationToken cancellationToken = default);
        Task<Seller?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedList<Seller>> GetAllAsync(SellerParameters sellerParameters , CancellationToken cancellationToken = default);
        Task<Seller?> UpdateAsync(Seller seller, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Seller>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Seller?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null, CancellationToken cancellationToken = default);
        Task<Seller?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        Task<bool> IsPhoneUniqueAsync(string phone, string? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> HasProductsAsync(string sellerId, CancellationToken cancellationToken = default);
        Task<long> GetProductCountAsync(string sellerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Seller>> GetByIdsAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default);
        Task<bool> AllExistAsync(IEnumerable<string> sellerIds, CancellationToken cancellationToken = default);
        Task<bool> UpdateEmailAsync(string sellerId, string newEmail, CancellationToken cancellationToken = default);
        Task<bool> UpdatePhoneAsync(string sellerId, string newPhone, CancellationToken cancellationToken = default);
        Task<PagedList<Seller>> GetAllSortedByNameAsync(int pageNumber, int pageSize, bool ascending = true, string? orderBy = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Seller>> GetRecentSellersAsync(int count, CancellationToken cancellationToken = default);
        Task<PagedList<Seller>> GetByCursorAsync(string? cursor, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default);
    }