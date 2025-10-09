using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.WarehouseDetail;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseDetailController : ControllerBase
{
    private readonly IWarehouseDetailService _service;

    public WarehouseDetailController(IWarehouseDetailService service)
    {
        _service = service;
    }

    // GET: api/WarehouseDetail/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var details = await _service.GetWarehouseDetailsByIdAsync(id);
        return Ok(details);
    }

    // GET: api/WarehouseDetail/by-warehouse/{warehouseId}
    [HttpGet("by-warehouse/{warehouseId:int}")]
    public async Task<IActionResult> GetByWarehouseId(int warehouseId)
    {
        var details = await _service.GetWarehouseDetailsByWarehouseIdAsync(warehouseId);
        return Ok(details);
    }

    // POST: api/WarehouseDetail
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WarehouseDetailCreateDto dto)
    {
        var created = await _service.CreateWarehouseDetailsAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/WarehouseDetail/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WarehouseDetailUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch.");

        var updated = await _service.UpdateWarehouseDetailsAsync(dto);
        return Ok(updated);
    }

    // DELETE: api/WarehouseDetail/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteWarehouseDetailsAsync(id);
        return NoContent();
    }
}