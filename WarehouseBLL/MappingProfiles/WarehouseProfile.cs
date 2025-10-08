using AutoMapper;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class WarehouseProfile : Profile
{
    public WarehouseProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();

        CreateMap<Warehouse, WarehouseWithDetailDto>()
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details.FirstOrDefault()));

        CreateMap<Warehouse, WarehouseWithInventoryDto>()
            .ForMember(dest => dest.Inventories, opt => opt.MapFrom(src => src.Inventories));

        CreateMap<WarehouseCreateDto, Warehouse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<WarehouseUpdateDto, Warehouse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) =>
            {
                var type = typeof(Warehouse);
                foreach (var prop in type.GetProperties())
                {
                    var srcProp = src.GetType().GetProperty(prop.Name);
                    if (srcProp != null)
                    {
                        var value = srcProp.GetValue(src);
                        if (value != null && prop.Name != "Id" && prop.Name != "CreatedAt" && 
                            prop.Name != "CreatedBy" && prop.Name != "IsDeleted" && 
                            prop.Name != "RowVersion" && prop.Name != "Inventories" && 
                            prop.Name != "Details")
                        {
                            prop.SetValue(dest, value);
                        }
                    }
                }
            });
    }
}