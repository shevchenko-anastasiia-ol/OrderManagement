using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Order;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _unitOfWork.OrderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetByIdAsync(long id)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
        if (order == null)
            throw new NotFoundException($"Order with ID {id} was not found.");

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> AddAsync(OrderCreateDto dto, string createdBy)
    {
        if (dto.OrderDate > DateTime.UtcNow)
            throw new ValidationException("Order date cannot be in the future.");

        var order = _mapper.Map<Order>(dto);
        order.CreatedAt = DateTime.UtcNow;
        order.CreatedBy = createdBy;

        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> UpdateAsync(OrderUpdateDto dto, string updatedBy)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {dto.OrderId} was not found.");

        _mapper.Map(dto, order);
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = updatedBy;

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<OrderDto>(order);
    }

    public async Task DeleteAsync(long id)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
        if (order == null)
            throw new NotFoundException($"Order with ID {id} was not found.");

        order.IsDeleted = true;
        order.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(long customerId)
    {
        var orders = await _unitOfWork.OrderRepository.GetOrdersByCustomerIdAsync(customerId);
        if (orders == null || !orders.Any())
            throw new NotFoundException($"No orders found for Customer ID {customerId}.");

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _unitOfWork.OrderRepository.GetOrdersByStatusAsync(status);
        if (orders == null || !orders.Any())
            throw new NotFoundException($"No orders found with status '{status}'.");

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetOrderWithDetailsAsync(long orderId)
    {
        var order = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {orderId} was not found.");

        return _mapper.Map<OrderDto>(order);
    }
}
