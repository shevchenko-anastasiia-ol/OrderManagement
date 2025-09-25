using MarketplaceDAL.Models;
using MarketplaceDAL.Repositories.Interfaces;

namespace MarketplaceDAL.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IEnumerable<Customer>> GetCustomersCreatedAfterAsync(DateTime date);
    Task<Customer> GetCustomerWithOrdersAsync(long customerId);
}