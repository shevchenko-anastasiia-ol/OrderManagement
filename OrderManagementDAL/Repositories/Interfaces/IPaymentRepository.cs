using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Payment entity, IDbTransaction? transaction = null);
        Task UpdateAsync(Payment entity, IDbTransaction? transaction = null);
        Task<Payment?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<Payment>> GetAllAsync(IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);

        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(long orderId, IDbTransaction? transaction = null);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status, IDbTransaction? transaction = null);
        Task<Payment?> GetLatestPaymentForOrderAsync(long orderId, IDbTransaction? transaction = null);
    }
}