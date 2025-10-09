using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.SupplierProduct;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class SupplierProductController : ControllerBase
    {
        private readonly ISupplierProductService _service;

        public SupplierProductController(ISupplierProductService service)
        {
            _service = service;
        }

        // GET: api/SupplierProduct/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetSupplierProductByIdAsync(id);
            return Ok(result);
        }

        // GET: api/SupplierProduct/supplier/{supplierId}
        [HttpGet("supplier/{supplierId:int}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var result = await _service.GetSupplierProductsBySupplierAsync(supplierId);
            return Ok(result);
        }

        // GET: api/SupplierProduct/product/{productId}
        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var result = await _service.GetSupplierProductsByProductAsync(productId);
            return Ok(result);
        }

        // GET: api/SupplierProduct/supplier/{supplierId}/product/{productId}
        [HttpGet("supplier/{supplierId:int}/product/{productId:int}")]
        public async Task<IActionResult> GetBySupplierAndProduct(int supplierId, int productId)
        {
            var result = await _service.GetSupplierProductAsync(supplierId, productId);
            return Ok(result);
        }

        // POST: api/SupplierProduct
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SupplierProductCreateDto dto)
        {
            var created = await _service.AddProductToSupplierAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // DELETE: api/SupplierProduct/supplier/{supplierId}/product/{productId}
        [HttpDelete("supplier/{supplierId:int}/product/{productId:int}")]
        public async Task<IActionResult> Remove(int supplierId, int productId)
        {
            await _service.RemoveProductFromSupplierAsync(supplierId, productId);
            return NoContent();
        }

        // GET: api/SupplierProduct/exists/supplier/{supplierId}/product/{productId}
        [HttpGet("exists/supplier/{supplierId:int}/product/{productId:int}")]
        public async Task<IActionResult> Exists(int supplierId, int productId)
        {
            var exists = await _service.SupplierProductExistsAsync(supplierId, productId);
            return Ok(exists);
        }
    }