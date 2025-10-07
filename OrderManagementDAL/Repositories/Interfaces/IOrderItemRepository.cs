using MarketplaceDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(OrderItem entity, CancellationToken ct = default);
        Task UpdateAsync(OrderItem entity, CancellationToken ct = default);
        Task<OrderItem?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<OrderItem>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);

        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId, CancellationToken ct = default);
        Task<IEnumerable<OrderItem>> GetCreatedAfterAsync(DateTime date, CancellationToken ct = default);
    }
}