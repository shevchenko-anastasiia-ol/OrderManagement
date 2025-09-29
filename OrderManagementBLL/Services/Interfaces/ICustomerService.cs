using OrderManagementBLL.DTOs.Customer;

namespace OrderManagementBLL.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto> GetCustomerByIdAsync(long id);
    Task<CustomerDto> AddCustomerAsync(CustomerCreateDto dto);
    Task<CustomerDto> UpdateCustomerAsync(CustomerUpdateDto dto);
    Task DeleteCustomerAsync(long id);
    Task<IEnumerable<CustomerDto>> GetCustomersCreatedAfterAsync(DateTime date);
    Task<CustomerDto> GetCustomerWithOrdersAsync(long customerId);
}