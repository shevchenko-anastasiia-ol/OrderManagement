using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Order;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services
{
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
            if (order == null || order.IsDeleted)
                throw new NotFoundException($"Order with ID {id} was not found.");

            return _mapper.Map<OrderDto>(order);
        }

        // --- Add з підтримкою idempotency ---
        public async Task<OrderDto> AddAsync(OrderCreateDto dto, string createdBy)
        {
            if (dto.OrderDate > DateTime.UtcNow)
                throw new ValidationException("Order date cannot be in the future.");

            // Idempotency за IdempotencyToken або іншим унікальним ключем
            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingOrder = await _unitOfWork.OrderRepository.GetByIdempotencyTokenAsync(dto.IdempotencyToken);
                if (existingOrder != null)
                    return _mapper.Map<OrderDto>(existingOrder);
            }

            var order = _mapper.Map<Order>(dto);
            order.CreatedAt = DateTime.UtcNow;
            order.CreatedBy = createdBy ?? "system";
            order.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderRepository.AddAsync(order, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<OrderDto>(order);
        }

        // --- Update з оптимістичною конкуренцією ---
        public async Task<OrderDto> UpdateAsync(OrderUpdateDto dto, string updatedBy)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null || order.IsDeleted)
                throw new NotFoundException($"Order with ID {dto.OrderId} was not found or deleted.");

            if (dto.RowVer != null && !order.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The order has been modified by another user.");

            _mapper.Map(dto, order);
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderRepository.UpdateAsync(order, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<OrderDto>(order);
        }

        // --- Delete з оптимістичною конкуренцією ---
        public async Task DeleteAsync(long id, byte[] rowVer = null, string updatedBy = null)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null || order.IsDeleted)
                throw new NotFoundException("Order is already deleted or does not exist.");

            if (rowVer != null && !order.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The order has been modified by another user.");

            order.IsDeleted = true;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderRepository.UpdateAsync(order, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // --- Спеціальні методи ---
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
            if (order == null || order.IsDeleted)
                throw new NotFoundException($"Order with ID {orderId} was not found.");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
