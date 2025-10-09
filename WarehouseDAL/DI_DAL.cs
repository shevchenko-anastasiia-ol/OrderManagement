using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseDAL.Data;
using WarehouseDAL.Repositories;
using WarehouseDAL.Repositories.Interfaces;
using WarehouseDAL.UnitOfWork;
using WarehouseDomain.Entities;

namespace WarehouseDAL;

public static class DI_DAL
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        // Підключення DbContext
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ISupplierProductRepository, SupplierProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IWarehouseDetailRepository, WarehouseDetailRepository>();

        return services;
    }
}