using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.Inventory;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/Inventory/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var inventory = await _inventoryService.GetInventoryWithDetailsAsync(id);
            return Ok(inventory);
        }

        // GET: api/Inventory/warehouse/{warehouseId}
        [HttpGet("warehouse/{warehouseId:int}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            var inventories = await _inventoryService.GetInventoryByWarehouseAsync(warehouseId);
            return Ok(inventories);
        }

        // GET: api/Inventory/product/{productId}
        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var inventories = await _inventoryService.GetInventoryByProductAsync(productId);
            return Ok(inventories);
        }

        // GET: api/Inventory/low-stock?threshold=10
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold)
        {
            var inventories = await _inventoryService.GetLowStockItemsAsync(threshold);
            return Ok(inventories);
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryCreateDto dto)
        {
            var created = await _inventoryService.CreateInventoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Inventory/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var updated = await _inventoryService.UpdateInventoryAsync(dto);
            return Ok(updated);
        }

        // PATCH: api/Inventory/adjust
        [HttpPatch("adjust")]
        public async Task<IActionResult> AdjustQuantity([FromBody] InventoryAdjustDto dto)
        {
            var updated = await _inventoryService.AdjustInventoryQuantityAsync(dto);
            return Ok(updated);
        }

        // DELETE: api/Inventory/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryService.DeleteInventoryAsync(id);
            return NoContent();
        }

        // GET: api/Inventory/exists/{id}
        [HttpGet("exists/{id:int}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _inventoryService.InventoryExistsAsync(id);
            return Ok(exists);
        }

        // GET: api/Inventory/total-stock/{productId}
        [HttpGet("total-stock/{productId:int}")]
        public async Task<IActionResult> GetTotalStock(int productId)
        {
            var total = await _inventoryService.GetTotalStockForProductAsync(productId);
            return Ok(total);
        }

        // GET: api/Inventory/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] InventoryQueryParams queryParams)
        {
            var pagedResult = await _inventoryService.GetInventoryPagedAsync(queryParams);
            return Ok(pagedResult);
        }
    }