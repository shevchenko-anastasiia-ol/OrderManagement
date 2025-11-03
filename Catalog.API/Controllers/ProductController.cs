using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Commands.Product.CreateProduct;
using Catalog.Application.Commands.Product.DeleteProduct;
using Catalog.Application.Commands.Product.UpdateProduct;
using Catalog.Application.Commands.Product.UpdateProductPrice;
using Catalog.Application.Commands.Product.AddProductImages;
using Catalog.Application.Commands.Product.AddCategoryToProduct;
using Catalog.Application.Commands.Product.RemoveCategoryFromProduct;
using Catalog.Application.Queries.Product.GetProductById;
using Catalog.Application.Queries.Product.GetProducts;
using Catalog.Application.Queries.Product.SearchProductsByText;
using Catalog.Application.Queries.Product.GetProductsByCategory;
using Catalog.Application.Queries.Product.GetProductsBySeller;
using Catalog.Application.Queries.Product.GetProductsByPriceRange;
using Catalog.Application.Queries.Product.GetLatestProducts;
using Catalog.Application.Queries.Product.GetProductsSortedByPrice;
using Catalog.Application.Queries.Product.GetProductsSortedByName;
using Catalog.Application.Queries.Product.GetProductPriceStatistics;
using Catalog.Application.Queries.Product.GetProductCountByCategory;
using Catalog.Application.Queries.Product.GetProductCountBySeller;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Exceptions;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        public ProductController(IMediator mediator) : base(mediator) { }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery { ProductId = id }, cancellationToken);
                if (product == null)
                    return NotFound();

                var etag = GenerateETag(product.UpdatedAt ?? product.CreatedAt);
                AddETagHeader(etag);

                return Ok(product);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductsQuery { Parameters = parameters };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createdProduct = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ProductId != id)
                    return BadRequest("ID mismatch");

                var requestETag = GetIfMatchHeader();
                var existingProduct = await _mediator.Send(new GetProductByIdQuery { ProductId = id }, cancellationToken);
                if (existingProduct == null)
                    return NotFound();

                var currentETag = GenerateETag(existingProduct.UpdatedAt ?? existingProduct.CreatedAt);
                if (!ValidateETag(requestETag, currentETag))
                    return StatusCode(412, new { message = "ETag mismatch. The resource has been modified." });

                var updatedProduct = await _mediator.Send(command, cancellationToken);

                var newETag = GenerateETag(updatedProduct.UpdatedAt ?? updatedProduct.CreatedAt);
                AddETagHeader(newETag);

                return Ok(updatedProduct);
            }
            catch (ConcurrencyException ex)
            {
                return HandleConcurrencyConflict(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PATCH: api/product/{id}/price
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(string id, [FromBody] UpdateProductPriceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ProductId != id)
                    return BadRequest("ID mismatch");

                var requestETag = GetIfMatchHeader();
                var existingProduct = await _mediator.Send(new GetProductByIdQuery { ProductId = id }, cancellationToken);
                if (existingProduct == null)
                    return NotFound();

                var currentETag = GenerateETag(existingProduct.UpdatedAt ?? existingProduct.CreatedAt);
                if (!ValidateETag(requestETag, currentETag))
                    return StatusCode(412, new { message = "ETag mismatch. The resource has been modified." });

                var updatedProduct = await _mediator.Send(command, cancellationToken);

                var newETag = GenerateETag(updatedProduct.UpdatedAt ?? updatedProduct.CreatedAt);
                AddETagHeader(newETag);

                return Ok(updatedProduct);
            }
            catch (ConcurrencyException ex)
            {
                return HandleConcurrencyConflict(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/product/{id}/images
        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddImages(string id, [FromBody] AddProductImagesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ProductId != id)
                    return BadRequest("ID mismatch");

                var updatedProduct = await _mediator.Send(command, cancellationToken);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] bool deleteReviews = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var command = new DeleteProductCommand { ProductId = id, DeleteReviews = deleteReviews };
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchByText([FromQuery] string searchTerm, [FromQuery] int? limit = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new SearchProductsByTextQuery { SearchTerm = searchTerm, Limit = limit };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductsByCategoryQuery { CategoryId = categoryId };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/seller/{sellerId}
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetBySeller(string sellerId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductsBySellerQuery { SellerId = sellerId };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/price-range
        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] string? currency = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetProductsByPriceRangeQuery { MinPrice = minPrice, MaxPrice = maxPrice, Currency = currency };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/latest
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestProducts([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetLatestProductsQuery { Count = count };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/sorted/price
        [HttpGet("sorted/price")]
        public async Task<IActionResult> GetSortedByPrice([FromQuery] bool ascending = true, [FromQuery] int? limit = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetProductsSortedByPriceQuery { Ascending = ascending, Limit = limit };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/sorted/name
        [HttpGet("sorted/name")]
        public async Task<IActionResult> GetSortedByName([FromQuery] bool ascending = true, [FromQuery] int? limit = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetProductsSortedByNameQuery { Ascending = ascending, Limit = limit };
                var products = await _mediator.Send(query, cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/product/{id}/categories/{categoryId}
        [HttpPost("{id}/categories/{categoryId}")]
        public async Task<IActionResult> AddCategory(string id, string categoryId, CancellationToken cancellationToken)
        {
            try
            {
                var command = new AddCategoryToProductCommand { ProductId = id, CategoryId = categoryId };
                await _mediator.Send(command, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // DELETE: api/product/{id}/categories/{categoryId}
        [HttpDelete("{id}/categories/{categoryId}")]
        public async Task<IActionResult> RemoveCategory(string id, string categoryId, CancellationToken cancellationToken)
        {
            try
            {
                var command = new RemoveCategoryFromProductCommand { ProductId = id, CategoryId = categoryId };
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/statistics/price
        [HttpGet("statistics/price")]
        public async Task<IActionResult> GetPriceStatistics([FromQuery] string? categoryId = null, [FromQuery] string? currency = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetProductPriceStatisticsQuery { CategoryId = categoryId, Currency = currency };
                var statistics = await _mediator.Send(query, cancellationToken);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/count/category
        [HttpGet("count/category")]
        public async Task<IActionResult> GetProductCountByCategory(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductCountByCategoryQuery();
                var counts = await _mediator.Send(query, cancellationToken);
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/product/count/seller
        [HttpGet("count/seller")]
        public async Task<IActionResult> GetProductCountBySeller(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductCountBySellerQuery();
                var counts = await _mediator.Send(query, cancellationToken);
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}