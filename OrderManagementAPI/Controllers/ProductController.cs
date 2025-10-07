using Microsoft.AspNetCore.Mvc;
using OrderManagementBLL.DTOs.Product;
using OrderManagementBLL.Exceptions;
using OrderManagementBLL.Services.Interfaces;
using System.Threading;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ProductDto>> GetById(long id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.AddAsync(dto, dto.CreatedBy);
                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
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

        // PUT: api/products/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<ProductDto>> Update(long id, ProductUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.ProductId)
                return BadRequest(new ProblemDetails { Title = "Product ID mismatch." });

            try
            {
                var updated = await _productService.UpdateAsync(dto, dto.UpdatedBy);
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

        // DELETE: api/products/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, [FromHeader] byte[] rowVer, [FromHeader] string updatedBy, CancellationToken cancellationToken)
        {
            try
            {
                await _productService.DeleteAsync(id, rowVer, updatedBy);
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

        // GET: api/products/by-price
        [HttpGet("by-price")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByPriceRange([FromQuery] decimal min, [FromQuery] decimal max, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetProductsByPriceRangeAsync(min, max);
                return Ok(products);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/products/in-stock
        [HttpGet("in-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetInStock(CancellationToken cancellationToken)
        {
            var products = await _productService.GetProductsInStockAsync();
            return Ok(products);
        }

        // GET: api/products/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> FindByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.FindProductsByNameAsync(name);
                return Ok(products);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // GET: api/products/count-in-stock
        [HttpGet("count-in-stock")]
        public async Task<ActionResult<int>> CountInStock(CancellationToken cancellationToken)
        {
            var count = await _productService.CountProductsInStockAsync();
            return Ok(count);
        }

        // GET: api/products/distinct-names
        [HttpGet("distinct-names")]
        public async Task<ActionResult<List<string>>> DistinctNames(CancellationToken cancellationToken)
        {
            var names = await _productService.GetDistinctProductNamesAsync();
            return Ok(names);
        }

        // GET: api/products/{id}/with-order-items
        [HttpGet("{id:long}/with-order-items")]
        public async Task<ActionResult<ProductDto>> GetWithOrderItems(long id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.GetProductWithOrderItemsAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
