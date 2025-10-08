using AutoMapper;
using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class SupplierProductProfile : Profile
{
    public SupplierProductProfile()
    {
        CreateMap<SupplierProduct, SupplierProductDto>();

        CreateMap<SupplierProductCreateDto, SupplierProduct>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));
    }
}