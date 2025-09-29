using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.OrderItem;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services;

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
        return _mapper.Map<OrderItemDto>(item);
    }

    public async Task<OrderItemDto> AddAsync(OrderItemCreateDto dto, string createdBy)
    {
        var item = _mapper.Map<OrderItem>(dto);
        item.CreatedAt = DateTime.UtcNow;
        item.CreatedBy = createdBy;

        await _unitOfWork.OrderItemRepository.AddAsync(item);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<OrderItemDto>(item);
    }

    public async Task<OrderItemDto> UpdateAsync(OrderItemUpdateDto dto, string updatedBy)
    {
        var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(dto.OrderItemId);
        if (item == null) return null;

        _mapper.Map(dto, item);
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = updatedBy;

        await _unitOfWork.OrderItemRepository.UpdateAsync(item);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<OrderItemDto>(item);
    }

    public async Task DeleteAsync(long id)
    {
        var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(id);
        if (item == null) return;

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.OrderItemRepository.UpdateAsync(item);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(long orderId)
    {
        var items = await _unitOfWork.OrderItemRepository.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<OrderItemDto>>(items);
    }

    public async Task<IEnumerable<OrderItemDto>> GetCreatedAfterAsync(DateTime date)
    {
        var items = await _unitOfWork.OrderItemRepository.GetCreatedAfterAsync(date);
        return _mapper.Map<IEnumerable<OrderItemDto>>(items);
    }
}