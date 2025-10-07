using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.Services.Interfaces;
using OrderManagementBLL.DTOs.Customer;
using OrderManagementBLL.Exceptions;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll(CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers); // 200 OK
        }

        // GET: api/customers/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CustomerDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                return Ok(customer); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(CustomerCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerService.AddCustomerAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer); // 201 Created
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

        // PUT: api/customers/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<CustomerDto>> Update(long id, CustomerUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.CustomerId)
                return BadRequest(new ProblemDetails { Title = "Customer ID mismatch." });

            try
            {
                var updated = await _customerService.UpdateCustomerAsync(dto);
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

        // DELETE: api/customers/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id, rowVer, updatedBy);
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

        // GET: api/customers/created-after?date=2025-10-01
        [HttpGet("created-after")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCreatedAfter([FromQuery] DateTime date, CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetCustomersCreatedAfterAsync(date);
            return Ok(customers); // 200 OK
        }

        // GET: api/customers/{id}/orders
        [HttpGet("{id:long}/orders")]
        public async Task<ActionResult<CustomerDto>> GetCustomerWithOrders(long id, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerService.GetCustomerWithOrdersAsync(id);
                return Ok(customer); // 200 OK
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message }); // 404
            }
        }
    }
}
