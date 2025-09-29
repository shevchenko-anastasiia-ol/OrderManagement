using OrderManagementBLL.DTOs.Payment;

namespace OrderManagementBLL.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto> GetByIdAsync(long id);
    Task<PaymentDto> AddAsync(PaymentCreateDto dto, string createdBy);
    Task<PaymentDto> UpdateAsync(PaymentUpdateDto dto, string updatedBy);
    Task DeleteAsync(long id);

    Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(long orderId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status);
    Task<PaymentDto> GetLatestPaymentForOrderAsync(long orderId);
}