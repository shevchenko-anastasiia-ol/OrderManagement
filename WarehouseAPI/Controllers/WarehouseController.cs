using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.Warehouse;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        // GET: api/Warehouse/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var warehouse = await _service.GetWarehouseByIdAsync(id);
            return Ok(warehouse);
        }

        // GET: api/Warehouse/{id}/details
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            var warehouse = await _service.GetWarehouseWithDetailsAsync(id);
            return Ok(warehouse);
        }

        // GET: api/Warehouse/{id}/inventory
        [HttpGet("{id:int}/inventory")]
        public async Task<IActionResult> GetWithInventory(int id)
        {
            var warehouse = await _service.GetWarehouseWithInventoryAsync(id);
            return Ok(warehouse);
        }

        // GET: api/Warehouse
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _service.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        // GET: api/Warehouse/with-details
        [HttpGet("with-details")]
        public async Task<IActionResult> GetAllWithDetails()
        {
            var warehouses = await _service.GetAllWarehousesWithDetailsAsync();
            return Ok(warehouses);
        }

        // GET: api/Warehouse/by-min-capacity/{minCapacity}
        [HttpGet("by-min-capacity/{minCapacity:int}")]
        public async Task<IActionResult> GetByMinCapacity(int minCapacity)
        {
            var warehouses = await _service.GetWarehousesByMinCapacityAsync(minCapacity);
            return Ok(warehouses);
        }

        // POST: api/Warehouse
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehouseCreateDto dto)
        {
            var created = await _service.CreateWarehouseAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Warehouse/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] WarehouseUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var updated = await _service.UpdateWarehouseAsync(dto);
            return Ok(updated);
        }

        // DELETE: api/Warehouse/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteWarehouseAsync(id);
            return NoContent();
        }
    }