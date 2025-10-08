using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseDetailService : IWarehouseDetailService
{
    private readonly IUnitOfWork _unitOfWork;

        public WarehouseDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WarehouseDetailDto?> GetWarehouseDetailsByIdAsync(int id)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(id);
            return details == null ? null : MapToViewDto(details);
        }

        public async Task<WarehouseDetailDto?> GetWarehouseDetailsByWarehouseIdAsync(int warehouseId)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(warehouseId);
            return details == null ? null : MapToViewDto(details);
        }

        public async Task<WarehouseDetailDto> CreateWarehouseDetailsAsync(WarehouseDetailCreateDto dto)
        {
            var existing = await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(dto.WarehouseId);
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Warehouse details already exist for Warehouse {dto.WarehouseId}");
            }

            var details = new WarehouseDetail
            {
                WarehouseId = dto.WarehouseId,
                Address = dto.Address,
                Manager = dto.Manager,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy,
                IsDeleted = false
            };

            await _unitOfWork.WarehouseDetailRepository.AddAsync(details);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(details);
        }

        public async Task<WarehouseDetailDto> UpdateWarehouseDetailsAsync(WarehouseDetailUpdateDto dto)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(dto.Id);
            if (details == null)
                throw new InvalidOperationException($"Warehouse details with ID {dto.Id} not found.");

            details.Address = dto.Address;
            details.Manager = dto.Manager;
            details.UpdatedAt = DateTime.UtcNow;
            details.UpdatedBy = dto.UpdatedBy;

            _unitOfWork.WarehouseDetailRepository.Update(details);
            await _unitOfWork.SaveChangesAsync();

            return MapToViewDto(details);
        }

        public async Task DeleteWarehouseDetailsAsync(int id)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(id);
            if (details != null)
            {
                details.IsDeleted = true;
                details.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.WarehouseDetailRepository.Update(details);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private WarehouseDetailDto MapToViewDto(WarehouseDetail details) => new()
        {
            Id = details.Id,
            WarehouseId = details.WarehouseId,
            Address = details.Address,
            Manager = details.Manager,
            CreatedAt = details.CreatedAt,
            UpdatedAt = details.UpdatedAt,
            IsDeleted = details.IsDeleted
        };
    }
