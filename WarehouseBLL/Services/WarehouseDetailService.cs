using AutoMapper;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Services.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseBLL.Services;

public class WarehouseDetailService : IWarehouseDetailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

        public WarehouseDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WarehouseDetailDto?> GetWarehouseDetailsByIdAsync(int id)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(id);
            return _mapper.Map<WarehouseDetailDto>(details);
        }

        public async Task<WarehouseDetailDto?> GetWarehouseDetailsByWarehouseIdAsync(int warehouseId)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(warehouseId);
            return _mapper.Map<WarehouseDetailDto>(details);
        }

        public async Task<WarehouseDetailDto> CreateWarehouseDetailsAsync(WarehouseDetailCreateDto dto)
        {
            var existing = await _unitOfWork.WarehouseDetailRepository.GetByWarehouseIdAsync(dto.WarehouseId);
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"Warehouse details already exist for Warehouse {dto.WarehouseId}");
            }

            var details = _mapper.Map<WarehouseDetail>(dto);

            await _unitOfWork.WarehouseDetailRepository.AddAsync(details);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseDetailDto>(details);
        }

        public async Task<WarehouseDetailDto> UpdateWarehouseDetailsAsync(WarehouseDetailUpdateDto dto)
        {
            var details = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(dto.Id);
            if (details == null)
                throw new InvalidOperationException($"Warehouse details with ID {dto.Id} not found.");

            _mapper.Map(dto, details);

            _unitOfWork.WarehouseDetailRepository.Update(details);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WarehouseDetailDto>(details);
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
