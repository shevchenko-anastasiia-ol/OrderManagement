using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.DTOs.Payment;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll(CancellationToken cancellationToken)
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        // GET: api/payments/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<PaymentDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                return Ok(payment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(PaymentCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _paymentService.AddAsync(dto, dto.CreatedBy);
                return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, payment);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message });
            }
        }

        // PUT: api/payments/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<PaymentDto>> Update(long id, PaymentUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.PaymentId)
                return BadRequest(new ProblemDetails { Title = "Payment ID mismatch." });

            try
            {
                var updated = await _paymentService.UpdateAsync(dto, dto.UpdatedBy);
                return Ok(updated);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // DELETE: api/payments/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _paymentService.DeleteAsync(id, rowVer, updatedBy);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/payments/by-order/{orderId}
        [HttpGet("by-order/{orderId:long}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrder(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                return Ok(payments);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/payments/by-status/{status}
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByStatus(string status, CancellationToken cancellationToken)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByStatusAsync(status);
                return Ok(payments);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/payments/latest/{orderId}
        [HttpGet("latest/{orderId:long}")]
        public async Task<ActionResult<PaymentDto>> GetLatestForOrder(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _paymentService.GetLatestPaymentForOrderAsync(orderId);
                return Ok(payment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
