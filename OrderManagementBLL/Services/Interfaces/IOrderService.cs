using OrderManagementBLL.DTOs.Order;

namespace OrderManagementBLL.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto> GetByIdAsync(long id);
    Task<OrderDto> AddAsync(OrderCreateDto dto, string createdBy);
    Task<OrderDto> UpdateAsync(OrderUpdateDto dto, string updatedBy);
    Task DeleteAsync(long id);

    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(long customerId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<OrderDto> GetOrderWithDetailsAsync(long orderId);
}