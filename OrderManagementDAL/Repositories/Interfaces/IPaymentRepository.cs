using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(long orderId);
    
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
    
    Task<Payment> GetLatestPaymentForOrderAsync(long orderId);
}