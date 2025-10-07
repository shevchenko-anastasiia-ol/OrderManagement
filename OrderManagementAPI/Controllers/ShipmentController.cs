using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.DTOs.Shipment;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        // GET: api/shipments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            var shipments = await _shipmentService.GetAllShipmentsAsync();
            return Ok(shipments);
        }

        // GET: api/shipments/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ShipmentDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var shipment = await _shipmentService.GetShipmentByIdAsync(id);
                return Ok(shipment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // POST: api/shipments
        [HttpPost]
        public async Task<ActionResult<ShipmentDto>> Create(ShipmentCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var shipment = await _shipmentService.AddShipmentAsync(dto, dto.CreatedBy);
                return CreatedAtAction(nameof(GetById), new { id = shipment.ShipmentId }, shipment);
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

        // PUT: api/shipments/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<ShipmentDto>> Update(long id, ShipmentUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.ShipmentId)
                return BadRequest(new ProblemDetails { Title = "Shipment ID mismatch." });

            try
            {
                var updated = await _shipmentService.UpdateShipmentAsync(dto, dto.UpdatedBy);
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

        // DELETE: api/shipments/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _shipmentService.DeleteShipmentAsync(id, rowVer, updatedBy);
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

        // GET: api/shipments/by-order/{orderId}
        [HttpGet("by-order/{orderId:long}")]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetByOrderId(long orderId, CancellationToken cancellationToken)
        {
            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderId);
            return Ok(shipments);
        }

        // GET: api/shipments/by-status
        [HttpGet("by-status")]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetByStatus([FromQuery] string status, CancellationToken cancellationToken)
        {
            try
            {
                var shipments = await _shipmentService.GetShipmentsByStatusAsync(status);
                return Ok(shipments);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/shipments/latest/{orderId}
        [HttpGet("latest/{orderId:long}")]
        public async Task<ActionResult<ShipmentDto>> GetLatestForOrder(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var shipment = await _shipmentService.GetLatestShipmentForOrderAsync(orderId);
                return Ok(shipment);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/shipments/count-by-carrier
        [HttpGet("count-by-carrier")]
        public async Task<ActionResult<int>> CountByCarrier([FromQuery] string carrier, CancellationToken cancellationToken)
        {
            try
            {
                var count = await _shipmentService.CountShipmentsByCarrierAsync(carrier);
                return Ok(count);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/shipments/distinct-carriers
        [HttpGet("distinct-carriers")]
        public async Task<ActionResult<List<string>>> DistinctCarriers(CancellationToken cancellationToken)
        {
            var carriers = await _shipmentService.GetDistinctCarriersAsync();
            return Ok(carriers);
        }
    }
}
