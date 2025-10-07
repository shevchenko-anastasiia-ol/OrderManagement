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
        Task AddAsync(Customer entity, CancellationToken ct = default);
        Task UpdateAsync(Customer entity, CancellationToken ct = default);
        Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetCustomersCreatedAfterAsync(DateTime date, CancellationToken ct = default);
        Task<Customer> GetCustomerWithOrdersAsync(long customerId, CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);
    }
}