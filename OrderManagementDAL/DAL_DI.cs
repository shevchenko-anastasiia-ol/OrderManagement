using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementDAL.Data;

namespace MarketplaceDAL;

public static class DAL_DI
{
    public static void RegisterDAL(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}