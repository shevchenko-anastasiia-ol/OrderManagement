using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Customer;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementBLL.Services
{
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
            var customers = await _unitOfWork.CustomerRepository.GetAllAsync(_unitOfWork.Transaction);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(long id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id, _unitOfWork.Transaction);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found.");

            return _mapper.Map<CustomerDto>(customer);
        }

        // --- Add з idempotency ---
        public async Task<CustomerDto> AddCustomerAsync(CustomerCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ValidationException("Customer first name is required.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ValidationException("Customer last name is required.");

            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingCustomer = await _unitOfWork.CustomerRepository
                    .GetByIdempotencyTokenAsync(dto.IdempotencyToken);

                if (existingCustomer != null)
                    return _mapper.Map<CustomerDto>(existingCustomer);
            }

            var customer = _mapper.Map<Customer>(dto);
            customer.CreatedAt = DateTime.UtcNow;
            customer.CreatedBy = dto.CreatedBy ?? "system";
            customer.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.CustomerRepository.AddAsync(customer, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        // --- Update з RowVer ---
        public async Task<CustomerDto> UpdateCustomerAsync(CustomerUpdateDto dto)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(dto.CustomerId, _unitOfWork.Transaction);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {dto.CustomerId} not found.");
            if (customer.IsDeleted)
                throw new BusinessConflictException("Cannot update a deleted customer.");
            if (dto.RowVer != null && !customer.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The record has been modified by another user.");

            _mapper.Map(dto, customer);
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = dto.UpdatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.CustomerRepository.UpdateAsync(customer, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        // --- Delete з RowVer ---
        public async Task DeleteCustomerAsync(long id, byte[] rowVer, string updatedBy)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id, _unitOfWork.Transaction);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {id} not found.");
            if (customer.IsDeleted)
                throw new BusinessConflictException("Customer is already deleted.");
            if (rowVer != null && !customer.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The record has been modified by another user.");

            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.CustomerRepository.UpdateAsync(customer, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersCreatedAfterAsync(DateTime date)
        {
            var customers = await _unitOfWork.CustomerRepository.GetCustomersCreatedAfterAsync(date, _unitOfWork.Transaction);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetCustomerWithOrdersAsync(long customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerWithOrdersAsync(customerId, _unitOfWork.Transaction);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {customerId} not found.");

            return _mapper.Map<CustomerDto>(customer);
        }
    }
}
