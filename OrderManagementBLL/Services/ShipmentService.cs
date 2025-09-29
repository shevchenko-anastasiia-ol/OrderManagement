using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Shipment;

namespace OrderManagementBLL.Services;

public class ShipmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ShipmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // --- CRUD ---
    public async Task<IEnumerable<ShipmentDto>> GetAllShipmentsAsync()
    {
        var shipments = await _unitOfWork.ShipmentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
    }

    public async Task<ShipmentDto> GetShipmentByIdAsync(long id)
    {
        var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(id);
        return _mapper.Map<ShipmentDto>(shipment);
    }

    public async Task<ShipmentDto> AddShipmentAsync(ShipmentCreateDto dto)
    {
        var shipment = _mapper.Map<Shipment>(dto);
        await _unitOfWork.ShipmentRepository.AddAsync(shipment);
        await _unitOfWork.SaveAsync();
        return _mapper.Map<ShipmentDto>(shipment);
    }

    public async Task<ShipmentDto> UpdateShipmentAsync(ShipmentUpdateDto dto)
    {
        var shipment = _mapper.Map<Shipment>(dto);
        await _unitOfWork.ShipmentRepository.UpdateAsync(shipment);
        await _unitOfWork.SaveAsync();
        return _mapper.Map<ShipmentDto>(shipment);
    }

    public async Task DeleteShipmentAsync(long id)
    {
        await _unitOfWork.ShipmentRepository.DeleteAsync(id);
        await _unitOfWork.SaveAsync();
    }

    // --- Спеціальні методи ---
    public async Task<IEnumerable<ShipmentDto>> GetShipmentsByOrderIdAsync(long orderId)
    {
        var shipments = await _unitOfWork.ShipmentRepository.GetShipmentsByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
    }

    public async Task<IEnumerable<ShipmentDto>> GetShipmentsByStatusAsync(string status)
    {
        var shipments = await _unitOfWork.ShipmentRepository.GetShipmentsByStatusAsync(status);
        return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
    }

    public async Task<ShipmentDto> GetLatestShipmentForOrderAsync(long orderId)
    {
        var shipment = await _unitOfWork.ShipmentRepository.GetLatestShipmentForOrderAsync(orderId);
        return _mapper.Map<ShipmentDto>(shipment);
    }

    public async Task<int> CountShipmentsByCarrierAsync(string carrier)
    {
        return await _unitOfWork.ShipmentRepository.CountShipmentsByCarrierAsync(carrier);
    }

    public async Task<List<string>> GetDistinctCarriersAsync()
    {
        return await _unitOfWork.ShipmentRepository.GetDistinctCarriersAsync();
    }
}