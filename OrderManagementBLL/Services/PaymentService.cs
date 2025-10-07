using AutoMapper;
using MarketplaceDAL.Models;
using MarketplaceDAL.UnitOfWork;
using OrderManagementBLL.DTOs.Payment;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;

namespace OrderManagementBLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // --- CRUD ---
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> GetByIdAsync(long id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null || payment.IsDeleted)
                throw new NotFoundException($"Payment with ID {id} was not found.");

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> AddAsync(PaymentCreateDto dto, string createdBy)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("Payment amount must be greater than zero.");

            // --- Idempotency ---
            if (!string.IsNullOrEmpty(dto.IdempotencyToken))
            {
                var existingPayment = await _unitOfWork.PaymentRepository
                    .GetByIdempotencyTokenAsync(dto.IdempotencyToken);

                if (existingPayment != null)
                    return _mapper.Map<PaymentDto>(existingPayment);
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.CreatedAt = DateTime.UtcNow;
            payment.CreatedBy = createdBy ?? "system";
            payment.IsDeleted = false;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> UpdateAsync(PaymentUpdateDto dto, string updatedBy)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(dto.PaymentId);
            if (payment == null || payment.IsDeleted)
                throw new NotFoundException($"Payment with ID {dto.PaymentId} was not found or deleted.");

            // --- Optimistic concurrency ---
            if (dto.RowVer != null && !payment.RowVer.SequenceEqual(dto.RowVer))
                throw new BusinessConflictException("The payment has been modified by another user.");

            _mapper.Map(dto, payment);
            payment.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task DeleteAsync(long id, byte[] rowVer, string updatedBy)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null || payment.IsDeleted)
                throw new NotFoundException("Payment is already deleted or does not exist.");

            // --- Optimistic concurrency ---
            if (rowVer != null && !payment.RowVer.SequenceEqual(rowVer))
                throw new BusinessConflictException("The payment has been modified by another user.");

            payment.IsDeleted = true;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedBy = updatedBy ?? "system";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // --- Спеціальні методи ---
        public async Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(long orderId)
        {
            var payments = await _unitOfWork.PaymentRepository.GetPaymentsByOrderIdAsync(orderId);
            if (payments == null || !payments.Any())
                throw new NotFoundException($"No payments found for Order ID {orderId}.");

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ValidationException("Status cannot be empty.");

            var payments = await _unitOfWork.PaymentRepository.GetPaymentsByStatusAsync(status);
            if (payments == null || !payments.Any())
                throw new NotFoundException($"No payments found with status '{status}'.");

            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> GetLatestPaymentForOrderAsync(long orderId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetLatestPaymentForOrderAsync(orderId);
            if (payment == null)
                throw new NotFoundException($"No payments found for Order ID {orderId}.");

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
