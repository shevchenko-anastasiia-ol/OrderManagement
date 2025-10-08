using AutoMapper;
using WarehouseBLL.DTOs.Inventory;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class InventoryProfile : Profile
{
    public InventoryProfile()
    {
        CreateMap<Inventory, InventoryDto>();

        CreateMap<Inventory, InventoryWithDetailsDto>()
            .ForMember(dest => dest.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

        CreateMap<InventoryCreateDto, Inventory>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<InventoryUpdateDto, Inventory>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) =>
            {
                if (src.Quantity > 0)
                    dest.Quantity = src.Quantity;
            });
    }
}