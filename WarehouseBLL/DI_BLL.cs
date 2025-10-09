using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseBLL.MappingProfiles;
using WarehouseBLL.Services;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseBLL;

public static class Bll_DI
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ISupplierProductService, SupplierProductService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IWarehouseDetailService, WarehouseDetailService>();
            
        services.AddAutoMapper(typeof(InventoryProfile).Assembly);

        return services;
    }
}