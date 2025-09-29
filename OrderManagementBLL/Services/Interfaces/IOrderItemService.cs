using OrderManagementBLL.DTOs.OrderItem;

namespace OrderManagementBLL.Services.Interfaces;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetAllAsync();
    Task<OrderItemDto> GetByIdAsync(long id);
    Task<OrderItemDto> AddAsync(OrderItemCreateDto dto, string createdBy);
    Task<OrderItemDto> UpdateAsync(OrderItemUpdateDto dto, string updatedBy);
    Task DeleteAsync(long id);
    Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(long orderId);
    Task<IEnumerable<OrderItemDto>> GetCreatedAfterAsync(DateTime date);
}