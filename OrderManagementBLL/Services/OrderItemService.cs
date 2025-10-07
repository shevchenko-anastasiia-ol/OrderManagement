using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.OrderItem;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var items = await _unitOfWork.OrderItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }

        public async Task<OrderItemDto> GetByIdAsync(long id)
        {
            var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(id);
            if (item == null || item.IsDeleted)
                throw new NotFoundException($"OrderItem with ID {id} was not found.");

            return _mapper.Map<OrderItemDto>(item);
        }

        // --- Add з idempotency ---
        public async Task<OrderItemDto> AddAsync(OrderItemCreateDto dto, string createdBy)
        {
            if (dto.Quantity <= 0)
                throw new ValidationException("Quantity must be greater than zero.");

            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingItem = await _unitOfWork.OrderItemRepository.GetByIdempotencyTokenAsync(dto.IdempotencyToken);
                if (existingItem != null)
                    return _mapper.Map<OrderItemDto>(existingItem);
            }

            var item = _mapper.Map<OrderItem>(dto);
            item.CreatedAt = DateTime.UtcNow;
            item.CreatedBy = createdBy ?? "system";
            item.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderItemRepository.AddAsync(item);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<OrderItemDto>(item);
        }

        // --- Update з RowVer ---
        public async Task<OrderItemDto> UpdateAsync(OrderItemUpdateDto dto, string updatedBy)
        {
            var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(dto.OrderItemId);
            if (item == null || item.IsDeleted)
                throw new NotFoundException($"OrderItem with ID {dto.OrderItemId} was not found or deleted.");

            if (dto.Quantity.HasValue && dto.Quantity <= 0)
                throw new ValidationException("Quantity must be greater than zero.");

            if (dto.RowVer != null && !item.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The record has been modified by another user.");

            _mapper.Map(dto, item);
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderItemRepository.UpdateAsync(item);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<OrderItemDto>(item);
        }

        // --- Delete з RowVer ---
        public async Task DeleteAsync(long id, byte[] rowVer, string updatedBy)
        {
            var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(id);
            if (item == null || item.IsDeleted)
                throw new NotFoundException("OrderItem is already deleted or does not exist.");

            if (rowVer != null && !item.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The record has been modified by another user.");

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.OrderItemRepository.UpdateAsync(item);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(long orderId)
        {
            var items = await _unitOfWork.OrderItemRepository.GetByOrderIdAsync(orderId);
            if (items == null || !items.Any())
                throw new NotFoundException($"No OrderItems found for Order ID {orderId}.");

            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }

        public async Task<IEnumerable<OrderItemDto>> GetCreatedAfterAsync(DateTime date)
        {
            var items = await _unitOfWork.OrderItemRepository.GetCreatedAfterAsync(date);
            return _mapper.Map<IEnumerable<OrderItemDto>>(items);
        }
    }
}
