using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Shipment;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services
{
    public class ShipmentService : IShipmentService
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
            if (shipment == null || shipment.IsDeleted)
                throw new NotFoundException($"Shipment with ID {id} not found.");

            return _mapper.Map<ShipmentDto>(shipment);
        }

        // --- Add з idempotency ---
        public async Task<ShipmentDto> AddShipmentAsync(ShipmentCreateDto dto, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(dto.Carrier))
                throw new ValidationException("Carrier cannot be empty.");
            if (dto.ShipmentDate == default)
                throw new ValidationException("Shipment date is required.");

            // Idempotency check
            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingShipment = await _unitOfWork.ShipmentRepository
                    .GetByIdempotencyTokenAsync(dto.IdempotencyToken);
                if (existingShipment != null)
                    return _mapper.Map<ShipmentDto>(existingShipment);
            }

            var shipment = _mapper.Map<Shipment>(dto);
            shipment.CreatedAt = DateTime.UtcNow;
            shipment.CreatedBy = createdBy ?? "system";
            shipment.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ShipmentRepository.AddAsync(shipment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<ShipmentDto>(shipment);
        }

        // --- Update з оптимістичною конкуренцією ---
        public async Task<ShipmentDto> UpdateShipmentAsync(ShipmentUpdateDto dto, string updatedBy)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(dto.ShipmentId);
            if (shipment == null || shipment.IsDeleted)
                throw new NotFoundException($"Shipment with ID {dto.ShipmentId} not found or deleted.");

            // Перевірка RowVer для оптимістичної конкуренції
            if (dto.RowVer != null && !shipment.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The shipment has been modified by another user.");

            _mapper.Map(dto, shipment);
            shipment.UpdatedAt = DateTime.UtcNow;
            shipment.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ShipmentRepository.UpdateAsync(shipment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<ShipmentDto>(shipment);
        }

        // --- Delete з оптимістичною конкуренцією ---
        public async Task DeleteShipmentAsync(long id, byte[] rowVer = null, string updatedBy = null)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(id);
            if (shipment == null || shipment.IsDeleted)
                throw new NotFoundException($"Shipment with ID {id} not found or already deleted.");

            if (rowVer != null && !shipment.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The shipment has been modified by another user.");

            shipment.IsDeleted = true;
            shipment.UpdatedAt = DateTime.UtcNow;
            shipment.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ShipmentRepository.UpdateAsync(shipment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // --- Спеціальні методи ---
        public async Task<IEnumerable<ShipmentDto>> GetShipmentsByOrderIdAsync(long orderId)
        {
            var shipments = await _unitOfWork.ShipmentRepository.GetShipmentsByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
        }

        public async Task<IEnumerable<ShipmentDto>> GetShipmentsByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ValidationException("Status cannot be empty.");

            var shipments = await _unitOfWork.ShipmentRepository.GetShipmentsByStatusAsync(status);
            return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
        }

        public async Task<ShipmentDto> GetLatestShipmentForOrderAsync(long orderId)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetLatestShipmentForOrderAsync(orderId);
            if (shipment == null)
                throw new NotFoundException($"No shipments found for order ID {orderId}.");

            return _mapper.Map<ShipmentDto>(shipment);
        }

        public async Task<int> CountShipmentsByCarrierAsync(string carrier)
        {
            if (string.IsNullOrWhiteSpace(carrier))
                throw new ValidationException("Carrier cannot be empty.");

            return await _unitOfWork.ShipmentRepository.CountShipmentsByCarrierAsync(carrier);
        }

        public async Task<List<string>> GetDistinctCarriersAsync()
        {
            return await _unitOfWork.ShipmentRepository.GetDistinctCarriersAsync();
        }
    }
}
