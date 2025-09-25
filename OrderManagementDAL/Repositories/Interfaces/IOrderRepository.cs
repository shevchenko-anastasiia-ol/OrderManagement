using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(long customerId);
    
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
    
    Task<Order> GetOrderWithDetailsAsync(long orderId);
}