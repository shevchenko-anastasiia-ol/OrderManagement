using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.Supplier;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        // GET: api/Supplier/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _service.GetSupplierByIdAsync(id);
            return Ok(supplier);
        }

        // GET: api/Supplier/with-products/{id}
        [HttpGet("with-products/{id:int}")]
        public async Task<IActionResult> GetWithProducts(int id)
        {
            var supplier = await _service.GetSupplierWithProductsAsync(id);
            return Ok(supplier);
        }

        // GET: api/Supplier/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _service.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET: api/Supplier/by-country
        [HttpGet("by-country")]
        public async Task<IActionResult> GetByCountry([FromQuery] string country)
        {
            var suppliers = await _service.GetSuppliersByCountryAsync(country);
            return Ok(suppliers);
        }

        // GET: api/Supplier/with-products
        [HttpGet("with-products")]
        public async Task<IActionResult> GetAllWithProducts()
        {
            var suppliers = await _service.GetAllSuppliersWithProductsAsync();
            return Ok(suppliers);
        }

        // POST: api/Supplier
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierCreateDto dto)
        {
            var created = await _service.CreateSupplierAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Supplier/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var updated = await _service.UpdateSupplierAsync(dto);
            return Ok(updated);
        }

        // DELETE: api/Supplier/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSupplierAsync(id);
            return NoContent();
        }

        // GET: api/Supplier/exists/{id}
        [HttpGet("exists/{id:int}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _service.SupplierExistsAsync(id);
            return Ok(exists);
        }

        // GET: api/Supplier/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] SupplierQueryParams queryParams)
        {
            var pagedResult = await _service.GetSuppliersPagedAsync(queryParams);
            return Ok(pagedResult);
        }
    }