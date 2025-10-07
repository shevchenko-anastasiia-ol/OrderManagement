using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.DTOs.OrderItem;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        // GET: api/orderitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _orderItemService.GetAllAsync();
            return Ok(items); // 200 OK
        }

        // GET: api/orderitems/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<OrderItemDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _orderItemService.GetByIdAsync(id);
                return Ok(item); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // POST: api/orderitems
        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> Create(OrderItemCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _orderItemService.AddAsync(dto, dto.CreatedBy);
                return CreatedAtAction(nameof(GetById), new { id = item.OrderItemId }, item); // 201 Created
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

        // PUT: api/orderitems/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<OrderItemDto>> Update(long id, OrderItemUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.OrderItemId)
                return BadRequest(new ProblemDetails { Title = "OrderItem ID mismatch." });

            try
            {
                var updated = await _orderItemService.UpdateAsync(dto, dto.UpdatedBy);
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

        // DELETE: api/orderitems/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _orderItemService.DeleteAsync(id, rowVer, updatedBy);
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

        // GET: api/orderitems/by-order/{orderId}
        [HttpGet("by-order/{orderId:long}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var items = await _orderItemService.GetByOrderIdAsync(orderId);
                return Ok(items); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // GET: api/orderitems/created-after?date=2025-10-01
        [HttpGet("created-after")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetCreatedAfter([FromQuery] DateTime date, CancellationToken cancellationToken)
        {
            var items = await _orderItemService.GetCreatedAfterAsync(date);
            return Ok(items); // 200 OK
        }
    }
}
