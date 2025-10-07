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
        Task AddAsync(OrderItem entity, IDbTransaction? transaction = null);
        Task UpdateAsync(OrderItem entity, IDbTransaction? transaction = null);
        Task<OrderItem?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<OrderItem>> GetAllAsync(IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);

        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId, IDbTransaction? transaction = null);
        Task<IEnumerable<OrderItem>> GetCreatedAfterAsync(DateTime date, IDbTransaction? transaction = null);
    }
}