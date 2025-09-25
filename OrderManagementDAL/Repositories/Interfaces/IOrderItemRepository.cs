using MarketplaceDAL.Models;

namespace MarketplaceDAL.Repositories.Interfaces;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderItem>> GetCreatedAfterAsync(DateTime date);
}