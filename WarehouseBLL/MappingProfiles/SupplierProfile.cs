using AutoMapper;
using WarehouseBLL.DTOs.Supplier;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class SupplierProfile : Profile
{
    public SupplierProfile()
    {
        CreateMap<Supplier, SupplierDto>();

        CreateMap<Supplier, SupplierWithProductDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => 
                src.SupplierProducts.Select(sp => sp.Product)));

        CreateMap<SupplierCreateDto, Supplier>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<SupplierUpdateDto, Supplier>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) =>
            {
                var type = typeof(Supplier);
                foreach (var prop in type.GetProperties())
                {
                    var srcProp = src.GetType().GetProperty(prop.Name);
                    if (srcProp != null)
                    {
                        var value = srcProp.GetValue(src);
                        if (value != null && prop.Name != "Id" && prop.Name != "CreatedAt" && 
                            prop.Name != "CreatedBy" && prop.Name != "IsDeleted" && 
                            prop.Name != "RowVersion" && prop.Name != "SupplierProducts")
                        {
                            prop.SetValue(dest, value);
                        }
                    }
                }
            });
    }
}