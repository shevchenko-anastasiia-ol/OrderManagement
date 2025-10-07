using MarketplaceDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Order entity, CancellationToken ct = default);
        Task UpdateAsync(Order entity, CancellationToken ct = default);
        Task<Order?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);

        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status, CancellationToken ct = default);

        Task<Order> GetOrderWithDetailsAsync(long orderId, CancellationToken ct = default);
    }
}