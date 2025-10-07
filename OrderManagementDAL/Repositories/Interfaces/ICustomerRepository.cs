using MarketplaceDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MarketplaceDAL.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdempotencyTokenAsync(string idempotencyToken);
        Task AddAsync(Customer entity, IDbTransaction? transaction = null);
        Task UpdateAsync(Customer entity, IDbTransaction? transaction = null);
        Task<Customer?> GetByIdAsync(long id, IDbTransaction? transaction = null);
        Task<IEnumerable<Customer>> GetAllAsync(IDbTransaction? transaction = null);
        Task<IEnumerable<Customer>> GetCustomersCreatedAfterAsync(DateTime date, IDbTransaction? transaction = null);
        Task<Customer> GetCustomerWithOrdersAsync(long customerId, IDbTransaction? transaction = null);
        Task DeleteAsync(long id, IDbTransaction? transaction = null);
    }
}