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
        Task AddAsync(Order entity, IDbTransaction? transaction = null);
        Task UpdateAsync(Order entity, IDbTransaction? transaction = null);
        Task<Order?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<Order>> GetAllAsync(IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);

        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId, IDbTransaction? transaction = null);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status, IDbTransaction? transaction = null);

        Task<Order> GetOrderWithDetailsAsync(long orderId, IDbTransaction? transaction = null);
    }
}