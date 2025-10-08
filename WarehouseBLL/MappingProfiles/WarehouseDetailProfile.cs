using AutoMapper;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class WarehouseDetailProfile : Profile
{
    public WarehouseDetailProfile()
    {
        CreateMap<WarehouseDetail, WarehouseDetailDto>();

        CreateMap<WarehouseDetailCreateDto, WarehouseDetail>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<WarehouseDetailUpdateDto, WarehouseDetail>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) =>
            {
                var type = typeof(WarehouseDetail);
                foreach (var prop in type.GetProperties())
                {
                    var srcProp = src.GetType().GetProperty(prop.Name);
                    if (srcProp != null)
                    {
                        var value = srcProp.GetValue(src);
                        if (value != null && prop.Name != "Id" && prop.Name != "WarehouseId" && 
                            prop.Name != "CreatedAt" && prop.Name != "CreatedBy" && 
                            prop.Name != "IsDeleted" && prop.Name != "RowVersion" && 
                            prop.Name != "Warehouse")
                        {
                            prop.SetValue(dest, value);
                        }
                    }
                }
            });
    }
}