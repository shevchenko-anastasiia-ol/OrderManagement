using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.DTOs.Order;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders); // 200 OK
        }

        // GET: api/orders/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<OrderDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(OrderCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.AddAsync(dto, dto.CreatedBy);
                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order); // 201 Created
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message }); // 400
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message }); // 409
            }
        }

        // PUT: api/orders/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<OrderDto>> Update(long id, OrderUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.OrderId)
                return BadRequest(new ProblemDetails { Title = "Order ID mismatch." });

            try
            {
                var updated = await _orderService.UpdateAsync(dto, dto.UpdatedBy);
                return Ok(updated); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message }); // 409
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message }); // 400
            }
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.DeleteAsync(id, rowVer, updatedBy);
                return NoContent(); // 204 No Content
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
            catch (BusinessConflictException ex)
            {
                return Conflict(new ProblemDetails { Title = ex.Message }); // 409
            }
        }

        // GET: api/orders/by-customer/{customerId}
        [HttpGet("by-customer/{customerId:long}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(long customerId, CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
                return Ok(orders); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // GET: api/orders/by-status/{status}
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(string status, CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                return Ok(orders); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // GET: api/orders/{id}/details
        [HttpGet("{id:long}/details")]
        public async Task<ActionResult<OrderDto>> GetWithDetails(long id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.GetOrderWithDetailsAsync(id);
                return Ok(order); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }
    }
}
