using Microsoft.AspNetCore.Mvc;
using WarehouseBLL.DTOs.Product;
using WarehouseBLL.Helpers;
using WarehouseBLL.Services.Interfaces;

namespace WarehouseAPI.Controllers;

[Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductWithInventoryAsync(id);
            return Ok(product);
        }

        // GET: api/Product/sku/{sku}
        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);
            return Ok(product);
        }

        // GET: api/Product/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/Product/price-range
        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(products);
        }

        // GET: api/Product/with-suppliers/{id}
        [HttpGet("with-suppliers/{id:int}")]
        public async Task<IActionResult> GetWithSuppliers(int id)
        {
            var product = await _productService.GetProductWithSuppliersAsync(id);
            return Ok(product);
        }

        // GET: api/Product/with-suppliers
        [HttpGet("with-suppliers")]
        public async Task<IActionResult> GetAllWithSuppliers()
        {
            var products = await _productService.GetProductsWithSuppliersAsync();
            return Ok(products);
        }

        // GET: api/Product/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var products = await _productService.GetLowStockProductsAsync(threshold);
            return Ok(products);
        }

        // GET: api/Product/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
        {
            var products = await _productService.GetRecentProductsAsync(count);
            return Ok(products);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var created = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Product/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var updated = await _productService.UpdateProductAsync(dto);
            return Ok(updated);
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        // GET: api/Product/exists/{id}
        [HttpGet("exists/{id:int}")]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _productService.ProductExistsAsync(id);
            return Ok(exists);
        }

        // GET: api/Product/sku-exists/{sku}
        [HttpGet("sku-exists/{sku}")]
        public async Task<IActionResult> SkuExists(string sku)
        {
            var exists = await _productService.SkuExistsAsync(sku);
            return Ok(exists);
        }

        // GET: api/Product/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] ProductQueryParams queryParams)
        {
            var pagedResult = await _productService.GetProductsPagedAsync(queryParams);
            return Ok(pagedResult);
        }
    }