using AutoMapper;
using WarehouseBLL.DTOs.Product;
using WarehouseDomain.Entities;

namespace WarehouseBLL.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<Product, ProductWithInventoryDto>()
            .ForMember(dest => dest.Inventories, opt => opt.MapFrom(src => src.Inventories));

        CreateMap<Product, ProductWithSuppliersDto>()
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src => 
                src.SupplierProducts.Select(sp => sp.Supplier)));

        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<ProductUpdateDto, Product>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .AfterMap((src, dest) =>
            {
                var type = typeof(Product);
                foreach (var prop in type.GetProperties())
                {
                    var srcProp = src.GetType().GetProperty(prop.Name);
                    if (srcProp != null)
                    {
                        var value = srcProp.GetValue(src);
                        if (value != null && prop.Name != "Id" && prop.Name != "CreatedAt" && 
                            prop.Name != "CreatedBy" && prop.Name != "IsDeleted" && 
                            prop.Name != "RowVersion" && prop.Name != "Inventories" && 
                            prop.Name != "SupplierProducts")
                        {
                            prop.SetValue(dest, value);
                        }
                    }
                }
            });
    }
}