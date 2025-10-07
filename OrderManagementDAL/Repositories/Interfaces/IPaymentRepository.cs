using MarketplaceDAL.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Payment entity, CancellationToken ct = default);
        Task UpdateAsync(Payment entity, CancellationToken ct = default);
        Task<Payment?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<Payment>> GetAllAsync(CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);

        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(long orderId, CancellationToken ct = default);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status, CancellationToken ct = default);
        Task<Payment?> GetLatestPaymentForOrderAsync(long orderId, CancellationToken ct = default);
    }
}