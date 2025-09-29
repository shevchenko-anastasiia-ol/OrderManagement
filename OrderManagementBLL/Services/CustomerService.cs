using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Customer;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _unitOfWork.CustomerRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }
    
    public async Task<CustomerDto> GetCustomerByIdAsync(long id)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new NotFoundException($"Customer with ID {id} not found.");

        return _mapper.Map<CustomerDto>(customer);
    }
    
    public async Task<CustomerDto> AddCustomerAsync(CustomerCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            throw new ValidationException("Customer first name is required.");
        if (string.IsNullOrWhiteSpace(dto.LastName))
            throw new ValidationException("Customer last name is required.");

        var customer = _mapper.Map<Customer>(dto);
        customer.CreatedAt = DateTime.UtcNow;
        customer.CreatedBy = "system"; // TODO: замінити на current user

        await _unitOfWork.CustomerRepository.AddAsync(customer);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CustomerDto>(customer);
    }
    
    public async Task<CustomerDto> UpdateCustomerAsync(CustomerUpdateDto dto)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null)
            throw new NotFoundException($"Customer with ID {dto.CustomerId} not found.");

        // можна додати бізнес-правила, наприклад, не можна оновити видаленого клієнта
        if (customer.IsDeleted)
            throw new BusinessConflictException("Cannot update a deleted customer.");

        _mapper.Map(dto, customer);
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = "system";

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CustomerDto>(customer);
    }
    
    public async Task DeleteCustomerAsync(long id)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new NotFoundException($"Customer with ID {id} not found.");

        if (customer.IsDeleted)
            throw new BusinessConflictException("Customer is already deleted.");

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = "system";

        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveAsync();
    }
    
    public async Task<IEnumerable<CustomerDto>> GetCustomersCreatedAfterAsync(DateTime date)
    {
        var customers = await _unitOfWork.CustomerRepository.GetCustomersCreatedAfterAsync(date);
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }
    
    public async Task<CustomerDto> GetCustomerWithOrdersAsync(long customerId)
    {
        var customer = await _unitOfWork.CustomerRepository.GetCustomerWithOrdersAsync(customerId);
        if (customer == null)
            throw new NotFoundException($"Customer with ID {customerId} not found.");

        return _mapper.Map<CustomerDto>(customer);
    }
}
