using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories;
using MarketplaceDAL.Repositories.Interfaces;
using MarketplaceDAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementDAL.Data;

namespace MarketplaceDAL;

public static class DAL_DI
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<OrderManagementDbContext>(options => 
            options.UseSqlServer(connectionString));
        
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<PaymentRepository, PaymentRepository>();
        services.AddScoped<ShipmentRepository, ShipmentRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}