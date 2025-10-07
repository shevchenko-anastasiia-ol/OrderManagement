using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementBLL.MappingProfiles;
using OrderManagementBLL.Services;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL
{
    public static class Bll_DI
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            
            services.AddAutoMapper(typeof(CustomerMappingProfile).Assembly);

            return services;
        }
    }
}